using System;
using System.Collections.Generic;
using System.Linq;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;

namespace DFDS.CapabilityService.Tests.TestDoubles
{
    public class StubDomainEventRegistrationManager : IDomainEventRegistrationManager
    {
        private readonly bool _stubbedIsRegistered;

        public StubDomainEventRegistrationManager(IEnumerable<Type> domainEventTypes = null, bool isRegisteredResult = false) 
            : this(ConvertToRegistrations(domainEventTypes ?? Enumerable.Empty<Type>()), isRegisteredResult)
        {
            
        }

        public StubDomainEventRegistrationManager(IEnumerable<DomainEventRegistration> stubbedRegistrations = null, bool stubbedIsRegistered = false)
        {
            Registrations = stubbedRegistrations ?? Enumerable.Empty<DomainEventRegistration>();
            _stubbedIsRegistered = stubbedIsRegistered;
        }

        private static IEnumerable<DomainEventRegistration> ConvertToRegistrations(IEnumerable<Type> domainEvenTypes)
        {
            return domainEvenTypes.Select(x => new DomainEventRegistrationBuilder()
                                               .WithEventInstanceType(x)
                                               .Build()
            );
        }

        public IEnumerable<DomainEventRegistration> Registrations { get; }

        public IDomainEventRegistrationManager Register<TEvent>(string eventTypeName, string topicName) where TEvent : IEvent
        {
            return this;
        }

        public bool IsRegistered(Type eventInstanceType)
        {
            return _stubbedIsRegistered;
        }
    }
}