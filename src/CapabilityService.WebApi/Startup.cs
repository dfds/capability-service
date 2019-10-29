using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using DFDS.CapabilityService.WebApi.Enablers.CorrelationId;
using DFDS.CapabilityService.WebApi.Enablers.KafkaStreaming;
using DFDS.CapabilityService.WebApi.Enablers.Metrics;
using DFDS.CapabilityService.WebApi.Enablers.PrometheusHealthCheck;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Configuration;
using DFDS.CapabilityService.WebApi.Features.Shared.Configuration;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;
using DFDS.CapabilityService.WebApi.Features.Topics.Configuration;

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
            var connectionString = Configuration["CAPABILITYSERVICE_DATABASE_CONNECTIONSTRING"];


            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCorrelationId();
            services.AddKafkaStreaming();
            services.AddMetrics();
            services
                .AddPrometheusHealthCheck()
                .AddNpgSql(connectionString, tags: new[] {"backing services", "postgres"});

            services.AddShared(connectionString);

            var eventRegistry = new DomainEventRegistry();
            services.AddSingleton<IDomainEventRegistry>(eventRegistry);
            services.AddSingleton(eventRegistry);


            var capabilitiesTopicName = Configuration["CAPABILITY_SERVICE_KAFKA_TOPIC_CAPABILITY"];
            services.AddCapabilities(
                eventRegistry,
                capabilitiesTopicName
            );

            var capabilitiesTopicsTopicName = Configuration["CAPABILITY_SERVICE_KAFKA_TOPIC_TOPICS"];
            services.AddTopics(
                eventRegistry,
                capabilitiesTopicsTopicName
            );

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