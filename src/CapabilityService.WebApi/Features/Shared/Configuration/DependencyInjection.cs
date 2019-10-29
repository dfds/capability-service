using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Features.Shared.Configuration
{
    public static class DependencyInjection
    {
        public static void AddShared(
            this IServiceCollection services, 
            string capabilityServiceDatabaseConnectionString
        )
        {
            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<CapabilityServiceDbContext>((serviceProvider, options) =>
                {
                    options.UseNpgsql(capabilityServiceDatabaseConnectionString);
                });

            services.AddTransient<EventHandlerFactory>();
      
            
            services.AddTransient<IEventDispatcher, EventDispatcher>();
            services.AddTransient<Outbox>();
            services.AddTransient<DomainEventEnvelopeRepository>();

      

            services.AddTransient<IRepository<DomainEventEnvelope>, DomainEventEnvelopeRepository>();

        }
    }
}