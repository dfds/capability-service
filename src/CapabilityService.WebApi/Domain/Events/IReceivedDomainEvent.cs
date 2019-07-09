using System;

namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    public interface IReceivedDomainEvent<T> : IEvent
    {
        string Version { get; }
        string EventName { get; }
        string XCorrelationId { get; }
        string XSender { get; }
        T Payload { get; }
    }
}