using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;
using Xunit;

namespace DFDS.CapabilityService.Tests.Infrastructure.Messaging
{
    public class TestOutbox
    {
        private class TestAggregateDomainEvent : IAggregateDomainEvents
        {
            private readonly ICollection<IDomainEvent> _events;
            public TestAggregateDomainEvent(List<IDomainEvent> list) => _events = list;

            public IEnumerable<IDomainEvent> DomainEvents => _events;
            public void ClearDomainEvents()
            {
                _events.Clear();
            }

            public string GetAggregateId()
            {
                return "AGGREGATE_STUB";
            }
        }

        private class TestDomainEventEnvelopeRepository : IRepository<DomainEventEnvelope>
        {
            public readonly List<DomainEventEnvelope> _list = new List<DomainEventEnvelope>();
            public Task Add(IEnumerable<DomainEventEnvelope> obj)
            {
                _list.AddRange(obj);
                return Task.CompletedTask;
            }

            public Task<IEnumerable<DomainEventEnvelope>> GetAll()
            {
                return Task.FromResult(_list.AsEnumerable());
            }
        }
        [Fact]
        public void test_queue_domain_events_retain_data()
        {
            var stubEvent = new FooDomainEvent();
            var stubRegistration = new DomainEventRegistrationBuilder()
                .WithEventInstanceType<FooDomainEvent>()
                .Build();
            var domainEventRegistry = new DomainEventRegistryBuilder().Build();
            domainEventRegistry.Register<FooDomainEvent>(stubRegistration.EventType, stubRegistration.Topic);
            
            var fakeRepository = new TestDomainEventEnvelopeRepository();
            var sut = new OutboxBuilder().WithRequestCorrelation().WithRepository(fakeRepository).WithRegistry(domainEventRegistry).Build();

            var testVal = new TestAggregateDomainEvent(new List<IDomainEvent>()
                {
                    new FooDomainEvent()
                }
                );
            sut.QueueDomainEvents(testVal);

            Assert.Single(fakeRepository._list);
            Assert.Equal("CORRELATION_STUB", fakeRepository._list[0].CorrelationId);
            Assert.Equal("AGGREGATE_STUB", fakeRepository._list[0].AggregateId);
        }

        private class FooDomainEvent : IDomainEvent { }
    }
}