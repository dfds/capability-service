using System;

namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    public interface IReceivedDomainEvent<T> : IEvent
    {
        string Version { get; }
        string EventName { get; }
        Guid XCorrelationId { get; }
        string XSender { get; }
        T Payload { get; }
    }
}