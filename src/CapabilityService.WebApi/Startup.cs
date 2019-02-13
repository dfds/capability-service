using System;
using System.Threading;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Infrastructure.Integrations;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;
using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<CapabilityServiceDbContext>((serviceProvider, options) =>
                {
                    var connectionString = Configuration["CAPABILITYSERVICE_DATABASE_CONNECTIONSTRING"];
                    options.UseNpgsql(connectionString);
                });

            services.AddHttpClient<IAMRoleServiceClient>(cfg =>
            {
                var baseUrl = Configuration["IAMROLESERVICE_URL"];
                if (baseUrl != null)
                {
                    cfg.BaseAddress = new Uri(baseUrl);
                }
            });

            services.AddHttpClient<RoleMapperServiceClient>(cfg =>
            {
                var baseUrl = Configuration["ROLEMAPPERSERVICE_URL"];
                if (baseUrl != null)
                {
                    cfg.BaseAddress = new Uri(baseUrl);
                }
            });

            services.AddTransient<ICapabilityRepository, CapabilityRepository>();
            services.AddTransient<IRoleService, RoleService>();

            services.AddTransient<Outbox>();
            services.AddTransient<DomainEventEnvelopRepository>();

            services.AddTransient<CapabilityApplicationService>();
            services.AddTransient<ICapabilityApplicationService>(serviceProvider => new CapabilityTransactionalDecorator(
                inner: serviceProvider.GetRequiredService<CapabilityApplicationService>(),
                dbContext: serviceProvider.GetRequiredService<CapabilityServiceDbContext>(),
                outbox: serviceProvider.GetRequiredService<Outbox>()
            ));

            ConfigureDomainEvents(services);
			services.AddHostedService<MetricHostedService>();
        }

        private static void ConfigureDomainEvents(IServiceCollection services)
        {
            var eventRegistry = new DomainEventRegistry();
            services.AddSingleton(eventRegistry);
            services.AddTransient<KafkaPublisherFactory.KafkaConfiguration>();
            services.AddTransient<KafkaPublisherFactory>();
            services.AddHostedService<PublishingService>();

            eventRegistry.Register<CapabilityCreated>("capabilitycreated", "build.capabilities");
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
}
