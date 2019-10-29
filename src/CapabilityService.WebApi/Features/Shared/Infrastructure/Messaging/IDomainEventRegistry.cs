using System;
using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging
{
    public interface IDomainEventRegistry : IDomainEventRegistrationManager
    {
        string GetTopicFor(string eventType);
        string GetTypeNameFor(IDomainEvent domainEvent);
        IEnumerable<string> GetAllTopics();
        Type GetInstanceTypeFor(string eventName);
    }
}