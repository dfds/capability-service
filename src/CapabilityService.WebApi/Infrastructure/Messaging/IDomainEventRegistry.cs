using DFDS.CapabilityService.WebApi.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public interface IDomainEventRegistry : IDomainEventRegistrationManager
    {
        string GetTopicFor(string eventType);
        string GetTypeNameFor(IDomainEvent domainEvent);
    }
}