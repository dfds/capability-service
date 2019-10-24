using DFDS.CapabilityService.WebApi.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Events
{
    public interface IIntegrationEvent<T> : IEvent
    {
        string Version { get; }
        string EventName { get; }
        string XCorrelationId { get; }
        string XSender { get; }
        T Payload { get; }
    }
}