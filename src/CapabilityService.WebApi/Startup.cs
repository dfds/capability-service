using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.EventHandlers;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Enablers.CorrelationId;
using DFDS.CapabilityService.WebApi.Enablers.KafkaStreaming;
using DFDS.CapabilityService.WebApi.Enablers.Metrics;
using DFDS.CapabilityService.WebApi.Enablers.PrometheusHealthCheck;
using DFDS.CapabilityService.WebApi.Infrastructure.Events;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;
using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using System.Reflection;
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
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddCorrelationId();
                
            var connectionString = Configuration["CAPABILITYSERVICE_DATABASE_CONNECTIONSTRING"];

            ConfigureApplicationServices(services, connectionString);
            services.AddKafkaStreaming();
            ConfigureDomainEvents(services);
            services.AddMetrics();

            services
                .AddPrometheusHealthCheck()
                .AddNpgSql(connectionString, tags: new[] {"backing services", "postgres"});

            services.AddAuthentication(options => { })
                    .AddAzureADBearer(options => Configuration.Bind("AzureAd", options));
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
           

            services.AddSingleton(eventRegistry);
            services.AddTransient<EventHandlerFactory>();
            services.AddTransient<IEventHandler<AWSContextAccountCreatedIntegrationEvent>, AWSContextAccountCreatedEventHandler>();
            services.AddTransient<IEventDispatcher, EventDispatcher>();

            var capabilitiesTopicName = Configuration["CAPABILITY_SERVICE_KAFKA_TOPIC_CAPABILITY"];
            var capabilitiesTopicsTopicName = Configuration["CAPABILITY_SERVICE_KAFKA_TOPIC_TOPICS"];

            eventRegistry
                .Register<CapabilityCreated>("capability_created", capabilitiesTopicName)
                .Register<CapabilityUpdated>("capability_updated", capabilitiesTopicName)
                .Register<CapabilityDeleted>("capability_deleted", capabilitiesTopicName)
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
            app.UseCorrelationId();
            
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
            
            app.UsePrometheusHealthCheck();
        }
    }
}
