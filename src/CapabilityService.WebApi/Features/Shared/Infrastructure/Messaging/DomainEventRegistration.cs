using System;

namespace DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging
{
    public class DomainEventRegistration
    {
        public DomainEventRegistration(string eventType, Type eventInstanceType, string topic)
        {
            EventType = eventType;
            EventInstanceType = eventInstanceType;
            Topic = topic;
        }

        public string EventType { get; }
        public Type EventInstanceType { get; }
        public string Topic { get; }
    }
}