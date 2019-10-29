using DFDS.CapabilityService.WebApi.Features.Capabilities.Application;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Infrastructure.EventHandlers;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Infrastructure.Events;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Infrastructure.Persistence;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.EventHandlers;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Features.Capabilities.Configuration
{
    public static class DependencyInjection
    {
        public static void AddCapabilities(
            this IServiceCollection services, 
            IDomainEventRegistry domainEventRegistry,
            string capabilitiesTopicName
        )
        {
            services.AddTransient<ICapabilityRepository, CapabilityRepository>();
            services.AddTransient<CapabilityOutboxEnabledDecorator>();
            services.AddTransient<CapabilityApplicationService>();
            services.AddTransient<ICapabilityApplicationService>(serviceProvider =>
                new CapabilityTransactionalDecorator(
                    inner: new CapabilityOutboxEnabledDecorator(
                        inner: serviceProvider.GetRequiredService<CapabilityApplicationService>(),
                        dbContext: serviceProvider.GetRequiredService<CapabilityServiceDbContext>(),
                        outbox: serviceProvider.GetRequiredService<Outbox>()
                    ),
                    dbContext: serviceProvider.GetRequiredService<CapabilityServiceDbContext>()
                ));
            
            services
                .AddTransient<IEventHandler<AWSContextAccountCreatedIntegrationEvent>,
                    AWSContextAccountCreatedEventHandler>();
            
            domainEventRegistry
                .Register<CapabilityCreated>("capability_created", capabilitiesTopicName)
                .Register<CapabilityUpdated>("capability_updated", capabilitiesTopicName)
                .Register<CapabilityDeleted>("capability_deleted", capabilitiesTopicName)
                .Register<MemberJoinedCapability>("member_joined_capability", capabilitiesTopicName)
                .Register<MemberLeftCapability>("member_left_capability", capabilitiesTopicName)
                .Register<ContextAddedToCapability>("context_added_to_capability", capabilitiesTopicName)
                .Register<ContextUpdated>("context_updated", capabilitiesTopicName)
                .Register<AWSContextAccountCreatedIntegrationEvent>("aws_context_account_created", capabilitiesTopicName);

        }
    }
}