using System;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class DomainEventRegistrationBuilder
    {
        private string _eventType;
        private Type _eventInstanceType;
        private string _topic;

        public DomainEventRegistrationBuilder()
        {
            _eventType = "foo";
            _eventInstanceType = typeof(DummyDomainEvent);
            _topic = "bar";
        }

        public DomainEventRegistrationBuilder WithEventType(string eventType)
        {
            _eventType = eventType;
            return this;
        }

        public DomainEventRegistrationBuilder WithEventInstanceType(Type eventInstanceType)
        {
            _eventInstanceType = eventInstanceType;
            return this;
        }

        public DomainEventRegistrationBuilder WithEventInstanceType<TEvent>() where TEvent : IDomainEvent
        {
            _eventInstanceType = typeof(TEvent);
            return this;
        }

        public DomainEventRegistrationBuilder WithEventInstanceType<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
        {
            _eventInstanceType = typeof(TEvent);
            return this;
        }

        public DomainEventRegistrationBuilder WithTopic(string topic)
        {
            _topic = topic;
            return this;
        }

        public DomainEventRegistration Build()
        {
            return new DomainEventRegistration(_eventType, _eventInstanceType, _topic);
        }

        private class DummyDomainEvent : IDomainEvent { }
    }
}