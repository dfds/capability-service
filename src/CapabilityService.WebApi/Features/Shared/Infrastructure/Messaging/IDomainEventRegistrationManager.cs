using System;
using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging
{
    public interface IDomainEventRegistrationManager
    {
        IEnumerable<DomainEventRegistration> Registrations { get; }
        IDomainEventRegistrationManager Register<TEvent>(string eventTypeName, string topicName) where TEvent : IEvent;
        bool IsRegistered(Type eventInstanceType);
    }
}