using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class Outbox
    {
        private readonly DomainEventEnvelopRepository _repository;
        private readonly DomainEventRegistry _eventRegistry;

        public Outbox(DomainEventEnvelopRepository repository, DomainEventRegistry eventRegistry)
        {
            _repository = repository;
            _eventRegistry = eventRegistry;
        }

        public Task QueueDomainEvents(IAggregateDomainEvents aggregate)
        {
            var domainEvents = aggregate
                               .DomainEvents
                               .Select(@event => new DomainEventEnvelop
                               {
                                   EventId = Guid.NewGuid(),
                                   AggregateId = aggregate.GetAggregateId(),
                                   Created = DateTime.UtcNow,
                                   Type = _eventRegistry.GetTypeNameFor(@event),
                                   Format = "application/json",
                                   Data = JsonConvert.SerializeObject(@event)
                               })
                               .ToArray();

            return QueueDomainEvents(domainEvents);
        }

        public async Task QueueDomainEvents(IEnumerable<DomainEventEnvelop> domainEvents)
        {
            await _repository.Add(domainEvents);
        }
    }

    public class DomainEventEnvelopRepository
    {
        private readonly CapabilityServiceDbContext _dbContext;

        public DomainEventEnvelopRepository(CapabilityServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(IEnumerable<DomainEventEnvelop> domainEvents)
        {
            await _dbContext.DomainEvents.AddRangeAsync(domainEvents);
        }
    }
}