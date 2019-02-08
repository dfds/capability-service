using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateDomainEvents
    {
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();

        protected AggregateRoot()
        {

        }

        protected AggregateRoot(TId id) : base(id)
        {

        }

        public IEnumerable<DomainEvent> DomainEvents => _domainEvents;

        protected void RaiseEvent<TEvent>(TEvent domainEvent) where TEvent : DomainEvent
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}