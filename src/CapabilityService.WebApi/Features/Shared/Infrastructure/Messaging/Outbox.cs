using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Enablers.CorrelationId;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging
{
    public class Outbox
    {
        private readonly IRepository<DomainEventEnvelope> _repository;
        private readonly IDomainEventRegistry _eventRegistry;
        private readonly IRequestCorrelation _requestCorrelation;

        public Outbox(IRepository<DomainEventEnvelope> repository, IDomainEventRegistry eventRegistry,
            IRequestCorrelation requestCorrelation)
        {
            _repository = repository;
            _eventRegistry = eventRegistry;
            _requestCorrelation = requestCorrelation;
        }

        public Task QueueDomainEvents(IAggregateDomainEvents aggregate)
        {
            var domainEvents = aggregate
                               .DomainEvents
                               .Select(@event => new DomainEventEnvelope
                               {
                                   EventId = Guid.NewGuid(),
                                   AggregateId = aggregate.GetAggregateId(),
                                   CorrelationId = _requestCorrelation.RequestCorrelationId,
                                   Created = DateTime.UtcNow,
                                   Type = _eventRegistry.GetTypeNameFor(@event),
                                   Format = "application/json",
                                   Data = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                                   {
                                       ContractResolver = new CamelCasePropertyNamesContractResolver()
                                   })
                               })
                               .ToArray();

            return QueueDomainEvents(domainEvents);
        }

        private async Task QueueDomainEvents(IEnumerable<DomainEventEnvelope> domainEvents)
        {
            await _repository.Add(domainEvents);
        }
    }
}