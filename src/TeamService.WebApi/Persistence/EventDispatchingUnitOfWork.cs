using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DFDS.TeamService.WebApi.Persistence
{
    public class EventDispatchingUnitOfWork<TDbContext> : UnitOfWork<TDbContext> where TDbContext : DbContext
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public EventDispatchingUnitOfWork(TDbContext dbContext, IDomainEventDispatcher domainEventDispatcher) : base(dbContext)
        {
            _domainEventDispatcher = domainEventDispatcher;
        }

        protected override async Task OnPostTransactionCommitted(IDbContextTransaction transaction)
        {
            var aggregates = DbContext.ChangeTracker
                .Entries<IAggregateDomainEvents>()
                .Select(x => x.Entity)
                .ToArray();

            foreach (var aggregate in aggregates)
            {
                await Dispatch(aggregate.DomainEvents);
                aggregate.ClearDomainEvents();
            }
        }

        private async Task Dispatch(IEnumerable<DomainEvent> domainEvents)
        {
            foreach (var @event in domainEvents)
            {
                await Dispatch((dynamic)@event);
            }
        }

        private async Task Dispatch<TEvent>(TEvent @event) where TEvent : DomainEvent
        {
            await _domainEventDispatcher.Dispatch(@event);
        }
    }
}