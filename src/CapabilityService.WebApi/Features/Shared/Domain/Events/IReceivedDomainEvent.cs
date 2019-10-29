namespace DFDS.CapabilityService.WebApi.Features.Shared.Domain.Events
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