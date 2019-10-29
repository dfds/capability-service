using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Features.Shared.Domain.Models
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateDomainEvents
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        protected AggregateRoot()
        {

        }

        protected AggregateRoot(TId id) : base(id)
        {

        }

        public IEnumerable<IDomainEvent> DomainEvents => _domainEvents;

        protected void RaiseEvent<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public string GetAggregateId()
        {
            return Id.ToString();
        }
    }
}