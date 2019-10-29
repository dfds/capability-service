using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Infrastructure.Persistence;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Persistence;
using DFDS.CapabilityService.WebApi.Features.Topics.Application;
using DFDS.CapabilityService.WebApi.Features.Topics.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Topics.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Features.Topics.Configuration
{
    public static class DependencyInjection
    {
        public static void AddTopics(
            this IServiceCollection services, 
            IDomainEventRegistry domainEventRegistry,
            string capabilitiesTopicsTopicName
        )
        {
            services.AddTransient<ITopicRepository, TopicRepository>();
            services.AddTransient<TopicApplicationService>();
            services.AddTransient<ITopicApplicationService>(serviceProvider => new TopicTransactionalDecorator(
                inner: serviceProvider.GetRequiredService<TopicApplicationService>(),
                dbContext: serviceProvider.GetRequiredService<CapabilityServiceDbContext>()
            ));
            

            domainEventRegistry.Register<TopicAdded>("topic_added", capabilitiesTopicsTopicName);
            
        }
    }
}