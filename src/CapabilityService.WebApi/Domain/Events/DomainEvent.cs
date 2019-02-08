using System;

namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    /// <summary>
    /// Marker interface for all domain events
    /// </summary>
    public abstract class DomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
    }
}