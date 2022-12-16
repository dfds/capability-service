using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Enablers.KafkaStreaming;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class DomainEventScanner
    {
        private readonly IDomainEventRegistrationManager _registrationManager;

        public DomainEventScanner(IDomainEventRegistrationManager registrationManager)
        {
            _registrationManager = registrationManager;
        }

        public IEnumerable<Type> GetDomainEventsFrom(Assembly assembly)
        {
            return GetDomainEventsFrom(assembly.GetTypes());
        }

        public IEnumerable<Type> GetDomainEventsFrom(IEnumerable<Type> types)
        {
            return types.Where(IsDomainEvent);
        }

        private bool IsDomainEvent(Type type)
        {
            if (type == typeof(IDomainEvent))
            {
                return false;
            }

            return type.IsClass && typeof(IDomainEvent).IsAssignableFrom(type)
                                && !typeof(IUseStandardContract).IsAssignableFrom(type);}

        public void EnsureNoUnregisteredDomainEventsIn(Assembly assembly)
        {
            var types = GetDomainEventsFrom(assembly);
            EnsureNoUnregisteredDomainEventsIn(types);
        }

        public void EnsureNoUnregisteredDomainEventsIn(IEnumerable<Type> types)
        {
            var notRegisteredEventTypes = types
                                          .Where(eventType => !_registrationManager.IsRegistered(eventType))
                                          .ToArray();

            if (notRegisteredEventTypes.Length > 0)
            {
                throw new MessagingException($"Error! The following domain events have not been registered: {string.Join(", ", notRegisteredEventTypes.Select(x => x.FullName))}");
            }
        }
    }
}
