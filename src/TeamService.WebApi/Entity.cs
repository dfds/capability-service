using System.Collections.Generic;
using DFDS.TeamService.WebApi.DomainEvents;

namespace DFDS.TeamService.WebApi
{
    public abstract class Entity<TId>
    {
        protected Entity()
        {
            
        }

        protected Entity(TId id)
        {
            Id = id;
        }

        public TId Id { get; protected set; }

        protected bool Equals(Entity<TId> other)
        {
            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Entity<TId>) obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TId>.Default.GetHashCode(Id);
        }
    }

    public interface IAggregateDomainEvents
    {
        IEnumerable<DomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }

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