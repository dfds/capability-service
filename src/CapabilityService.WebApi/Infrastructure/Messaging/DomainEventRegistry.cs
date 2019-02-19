using System;
using System.Collections.Generic;
using System.Linq;
using DFDS.CapabilityService.WebApi.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class DomainEventRegistry
    {
        private readonly List<DomainEventRegistration> _registrations = new List<DomainEventRegistration>();

        public IEnumerable<DomainEventRegistration> Registrations => _registrations;

        public DomainEventRegistry Register<TEvent>(string eventTypeName, string topicName) where TEvent : IDomainEvent
        {
            _registrations.Add(new DomainEventRegistration
            (
                eventType: eventTypeName,
                eventInstanceType: typeof(TEvent),
                topic: topicName
            ));

            return this;
        }

        public string GetTopicFor(string eventType)
        {
            var registration = _registrations.SingleOrDefault(x => x.EventType == eventType);

            if (registration == null)
            {
                throw new MessagingException($"Error! Could not determine \"topic name\" due to no registration was found for event type \"{eventType}\"!");
            }

            return registration.Topic;
        }

        public string GetTypeNameFor(IDomainEvent domainEvent)
        {
            var registration = _registrations.SingleOrDefault(x => x.EventInstanceType == domainEvent.GetType());

            if (registration == null)
            {
                throw new MessagingException($"Error! Could not determine \"event type name\" due to no registration was found for type {domainEvent.GetType().FullName}!");
            }

            return registration.EventType;
        }
    }

    public class DomainEventRegistration
    {
        public DomainEventRegistration(string eventType, Type eventInstanceType, string topic)
        {
            EventType = eventType;
            EventInstanceType = eventInstanceType;
            Topic = topic;
        }

        public string EventType { get; }
        public Type EventInstanceType { get; }
        public string Topic { get; }
    }
}