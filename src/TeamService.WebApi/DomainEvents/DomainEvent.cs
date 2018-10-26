using System;

namespace DFDS.TeamService.WebApi.DomainEvents
{
    /// <summary>
    /// Marker interface for all domain events
    /// </summary>
    public abstract class DomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
    }
}