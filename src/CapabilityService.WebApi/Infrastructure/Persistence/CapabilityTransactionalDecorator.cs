using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Newtonsoft.Json;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Persistence
{
    public class CapabilityTransactionalDecorator : ICapabilityApplicationService
    {
        private readonly ICapabilityApplicationService _inner;
        private readonly CapabilityServiceDbContext _dbContext;
        private readonly Outbox _outbox;

        public CapabilityTransactionalDecorator(ICapabilityApplicationService inner, CapabilityServiceDbContext dbContext, Outbox outbox)
        {
            _inner = inner;
            _dbContext = dbContext;
            _outbox = outbox;
        }

        public Task<IEnumerable<Capability>> GetAllCapabilities() => _inner.GetAllCapabilities();
        public Task<Capability> GetCapability(Guid id) => _inner.GetCapability(id);

        public async Task<Capability> CreateCapability(string name)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var capability = await _inner.CreateCapability(name);

                var aggregates = _dbContext
                                 .ChangeTracker
                                 .Entries<IAggregateDomainEvents>()
                                 .Select(x => x.Entity)
                                 .ToArray();

                foreach (var aggregate in aggregates)
                {
                    await _outbox.QueueDomainEvents(aggregate);
                }

                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                return capability;
            }
        }

        public async Task JoinCapability(Guid capabilityId, string memberEmail)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await _inner.JoinCapability(capabilityId, memberEmail);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }

        public async Task LeaveCapability(Guid capabilityId, string memberEmail)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await _inner.LeaveCapability(capabilityId, memberEmail);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }
    }

    public class DomainEventEnvelop
    {
        public Guid EventId { get; set; }
        public string AggregateId { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string Data { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Sent { get; set; }
    }

    public class Outbox
    {
        private readonly CapabilityServiceDbContext _dbContext;

        public Outbox(CapabilityServiceDbContext dbContext)
        {
            _dbContext = dbContext;
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
                                   Type = ExtractEventTypeFrom(@event),
                                   Format = "application/json",
                                   Data = JsonConvert.SerializeObject(@event)
                               })
                               .ToArray();

            return QueueDomainEvents(domainEvents);
        }

        private string ExtractEventTypeFrom(IDomainEvent @event)
        {
            return @event.GetType().Name.ToLower();
        }

        public async Task QueueDomainEvents(IEnumerable<DomainEventEnvelop> domainEvents)
        {
            await _dbContext.DomainEvents.AddRangeAsync(domainEvents);
        }
    }
}