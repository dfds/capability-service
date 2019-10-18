using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using CorrelationId;
using System.Net.Http;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.EventHandlers;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Enablers.Metrics;
using DFDS.CapabilityService.WebApi.Infrastructure.Events;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;
using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;


namespace DFDS.CapabilityService.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddHttpContextAccessor();
            services.AddCorrelationId();
            services.AddTransient<CorrelationIdRequestAppendHandler>();
            services.AddScoped<IRequestCorrelation, RequestCorrelation>();


            var connectionString = Configuration["CAPABILITYSERVICE_DATABASE_CONNECTIONSTRING"];

            ConfigureApplicationServices(services, connectionString);
            ConfigureDomainEvents(services);
            services.AddMetricsDependencies();

			// health checks
            var health = services
                .AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddNpgSql(connectionString, tags: new[] {"backing services", "postgres"});
        }

        private void ConfigureApplicationServices(IServiceCollection services, string connectionString)
        {
            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<CapabilityServiceDbContext>((serviceProvider, options) =>
                {
                    options.UseNpgsql(connectionString);
                });

            services.AddTransient<Outbox>();
            services.AddTransient<DomainEventEnvelopeRepository>();

            services.AddTransient<ICapabilityRepository, CapabilityRepository>();
            services.AddTransient<CapabilityOutboxEnabledDecorator>();
            services.AddTransient<CapabilityApplicationService>();
            services.AddTransient<ICapabilityApplicationService>(serviceProvider => new CapabilityTransactionalDecorator(
                inner: new CapabilityOutboxEnabledDecorator(
                    inner: serviceProvider.GetRequiredService<CapabilityApplicationService>(),
                    dbContext: serviceProvider.GetRequiredService<CapabilityServiceDbContext>(),
                    outbox: serviceProvider.GetRequiredService<Outbox>()
                ),
                dbContext: serviceProvider.GetRequiredService<CapabilityServiceDbContext>()
            ));

            services.AddTransient<ITopicRepository, TopicRepository>();
            services.AddTransient<TopicApplicationService>();
            services.AddTransient<ITopicApplicationService>(serviceProvider => new TopicTransactionalDecorator(
                inner: serviceProvider.GetRequiredService<TopicApplicationService>(),
                dbContext: serviceProvider.GetRequiredService<CapabilityServiceDbContext>()
            ));

            services.AddTransient<IRepository<DomainEventEnvelope>,DomainEventEnvelopeRepository>();
        }

        private void ConfigureDomainEvents(IServiceCollection services)
        {
            var eventRegistry = new DomainEventRegistry();
            services.AddSingleton<IDomainEventRegistry>(eventRegistry);
            services.AddTransient<KafkaConfiguration>();
            services.AddTransient<KafkaPublisherFactory>();
            services.AddTransient<KafkaConsumerFactory>();
            services.AddHostedService<PublishingService>();
            services.AddHostedService<ConsumerHostedService>();


            services.AddSingleton(eventRegistry);
            services.AddTransient<EventHandlerFactory>();
            services.AddTransient<IEventHandler<AWSContextAccountCreatedIntegrationEvent>, AWSContextAccountCreatedEventHandler>();
            services.AddTransient<IEventDispatcher, EventDispatcher>();

            var capabilitiesTopicName = Configuration["CAPABILITY_SERVICE_KAFKA_TOPIC_CAPABILITY"];
            var capabilitiesTopicsTopicName = Configuration["CAPABILITY_SERVICE_KAFKA_TOPIC_TOPICS"];

            eventRegistry
                .Register<CapabilityCreated>("capability_created", capabilitiesTopicName)
                .Register<CapabilityUpdated>("capability_updated", capabilitiesTopicName)
                .Register<MemberJoinedCapability>("member_joined_capability", capabilitiesTopicName)
                .Register<MemberLeftCapability>("member_left_capability", capabilitiesTopicName)
                .Register<ContextAddedToCapability>("context_added_to_capability", capabilitiesTopicName)
                .Register<ContextUpdated>("context_updated", capabilitiesTopicName)
                .Register<AWSContextAccountCreatedIntegrationEvent>("aws_context_account_created", capabilitiesTopicName)
                .Register<TopicAdded>("topic_added", capabilitiesTopicsTopicName);

            var scanner = new DomainEventScanner(eventRegistry);
            scanner.EnsureNoUnregisteredDomainEventsIn(Assembly.GetExecutingAssembly());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = "x-correlation-id",
                UpdateTraceIdentifier = true,
                IncludeInResponse = true,
                UseGuidForCorrelationId = true
            });     

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpMetrics();

            app.UseMvc();
            

            app.UseHealthChecks("/healthz", new HealthCheckOptions
            {
                ResponseWriter = MyPrometheusStuff.WriteResponseAsync
            });
        }
    }
    
    public class CorrelationIdRequestAppendHandler : DelegatingHandler
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public CorrelationIdRequestAppendHandler(ICorrelationContextAccessor correlationContextAccessor)
        {
            _correlationContextAccessor = correlationContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headerName = _correlationContextAccessor.CorrelationContext.Header;
            var correlationId = _correlationContextAccessor.CorrelationContext.CorrelationId;

            if (!request.Headers.Contains(headerName))
            {
                request.Headers.Add(headerName, correlationId);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }

    public static class MyPrometheusStuff
    {
        private const string HealthCheckLabelServiceName = "service";
        private const string HealthCheckLabelStatusName = "status";

        private static readonly Gauge HealthChecksDuration;
        private static readonly Gauge HealthChecksResult;

        static MyPrometheusStuff()
        {
            HealthChecksResult = Metrics.CreateGauge("healthcheck",
                "Shows health check status (status=unhealthy|degraded|healthy) 1 for triggered, otherwise 0", new GaugeConfiguration
                {
                    LabelNames = new[] {HealthCheckLabelServiceName, HealthCheckLabelStatusName},
                    SuppressInitialValue = false
                });

            HealthChecksDuration = Metrics.CreateGauge("healthcheck_duration_seconds",
                "Shows duration of the health check execution in seconds",
                new GaugeConfiguration
                {
                    LabelNames = new[] {HealthCheckLabelServiceName},
                    SuppressInitialValue = false
                });
        }

        public static Task WriteResponseAsync(HttpContext httpContext, HealthReport healthReport)
        {
            UpdateMetrics(healthReport);

            httpContext.Response.ContentType = "text/plain";
            return httpContext.Response.WriteAsync(healthReport.Status.ToString());
        }

        private static void UpdateMetrics(HealthReport report)
        {
            foreach (var (key, value) in report.Entries)
            {
                HealthChecksResult.Labels(key, "healthy").Set(value.Status == HealthStatus.Healthy ? 1 : 0);
                HealthChecksResult.Labels(key, "unhealthy").Set(value.Status == HealthStatus.Unhealthy ? 1 : 0);
                HealthChecksResult.Labels(key, "degraded").Set(value.Status == HealthStatus.Degraded ? 1 : 0);

                HealthChecksDuration.Labels(key).Set(value.Duration.TotalSeconds);
            }
        }
    }
}
