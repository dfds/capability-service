using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Infrastructure.Integrations;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;
using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Prometheus;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

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
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var connectionString = Configuration["CAPABILITYSERVICE_DATABASE_CONNECTIONSTRING"];
            
            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<CapabilityServiceDbContext>((serviceProvider, options) =>
                {
                    options.UseNpgsql(connectionString);
                });

            services.AddHttpClient<IAMRoleServiceClient>(cfg =>
            {
                var url = Configuration["IAMROLESERVICE_URL"];
                if (url != null)
                {
                    cfg.BaseAddress = new Uri(url);

                }
            });

            services.AddHttpClient<RoleMapperServiceClient>(cfg =>
            {
                var url = Configuration["ROLEMAPPERSERVICE_URL"];
                if (url != null)
                {
                    cfg.BaseAddress = new Uri(url);
                }
            });

            services.AddTransient<ICapabilityRepository, CapabilityRepository>();
            services.AddTransient<IRoleService, RoleService>();

            services.AddTransient<Outbox>();
            services.AddTransient<DomainEventEnvelopRepository>();

            services.AddTransient<CapabilityApplicationService>();
            services.AddTransient<CapabilityOutboxEnabledDecorator>();

            services.AddTransient<ICapabilityApplicationService>(serviceProvider => new CapabilityTransactionalDecorator(
                    inner: new CapabilityOutboxEnabledDecorator(
                        inner: serviceProvider.GetRequiredService<CapabilityApplicationService>(),
                        dbContext: serviceProvider.GetRequiredService<CapabilityServiceDbContext>(),
                        outbox: serviceProvider.GetRequiredService<Outbox>()
                    ),
                    dbContext: serviceProvider.GetRequiredService<CapabilityServiceDbContext>()
                ));

            ConfigureDomainEvents(services);
			services.AddHostedService<MetricHostedService>();
			
			// health checks
            var health = services
                .AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddNpgSql(connectionString, tags: new[] {"backing services", "postgres"});
        }

        private static void ConfigureDomainEvents(IServiceCollection services)
        {
            var eventRegistry = new DomainEventRegistry();
            services.AddSingleton<IDomainEventRegistry>(eventRegistry);
            services.AddTransient<KafkaPublisherFactory.KafkaConfiguration>();
            services.AddTransient<KafkaPublisherFactory>();
            services.AddHostedService<PublishingService>();

            var capabilitiesTopicName = "build.capabilities";

            eventRegistry
                .Register<CapabilityCreated>("capability_created", capabilitiesTopicName)
                .Register<MemberJoinedCapability>("member_joined_capability", capabilitiesTopicName)
                .Register<MemberLeftCapability>("member_left_capability", capabilitiesTopicName)
                .Register<ContextAddedToCapability>("context_added_to_capability", capabilitiesTopicName);

            var scanner = new DomainEventScanner(eventRegistry);
            scanner.EnsureNoUnregisteredDomainEventsIn(Assembly.GetExecutingAssembly());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMvc();
            
            app.UseHealthChecks("/healthz", new HealthCheckOptions
            {
                ResponseWriter = MyPrometheusStuff.WriteResponseAsync
            });
        }
    }

    public class MetricHostedService : IHostedService
    {
        private const string Host = "0.0.0.0";
        private const int Port = 8080;

        private IMetricServer _metricServer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Staring metric server on {Host}:{Port}");

            _metricServer = new KestrelMetricServer(Host, Port).Start();

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using (_metricServer)
            {
                Console.WriteLine("Shutting down metric server");
                await _metricServer.StopAsync();
                Console.WriteLine("Done shutting down metric server");
            }
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
