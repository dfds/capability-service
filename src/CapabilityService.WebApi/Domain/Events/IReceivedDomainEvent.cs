using System;

namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    public interface IReceivedDomainEvent<T>
    {
        string Version { get; }
        string EventName { get; }
        Guid XCorrelationId { get; }
        string XSender { get; }
        T Payload { get; }
    }
}