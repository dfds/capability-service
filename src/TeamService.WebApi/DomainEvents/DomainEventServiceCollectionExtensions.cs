using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.TeamService.WebApi.DomainEvents
{
    public static class DomainEventServiceCollectionExtensions
    {
        private static IEnumerable<Type> GetClosingImplementations(this Type type, Type openGenericInterface)
        {
            if (!openGenericInterface.IsGenericTypeDefinition)
            {
                return Enumerable.Empty<Type>();
            }
            
            return type
                .GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == openGenericInterface);
        }

        private static bool IsImplementingInterface(this Type type, Type interfaceType)
        {
            if (!interfaceType.IsGenericTypeDefinition)
            {
                return interfaceType.IsAssignableFrom(type);
            }

            return GetClosingImplementations(type, interfaceType).Any();
        }

        public static void AddDomainEventSubscribers(this IServiceCollection serviceCollection)
        {
            var subscriberImplementationTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsClass)
                .Where(x => x.IsPublic)
                .Where(x => x.IsImplementingInterface(typeof(IDomainEventSubscriber<>)));

            foreach (var subscriberType in subscriberImplementationTypes)
            {
                var serviceTypes = subscriberType.GetClosingImplementations(typeof(IDomainEventSubscriber<>));

                foreach (var serviceType in serviceTypes)
                {
                    serviceCollection.AddTransient(serviceType, subscriberType);
                }
            }
        }

        public static void AddDomainEvents(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDomainEventSubscribers();
            serviceCollection.AddTransient<IEventSubscriberProvider, ContainerBasedEventSubscriberProvider>();
            serviceCollection.AddTransient<ImmediateDomainEventDispatcher>();
            serviceCollection.AddTransient<IDomainEventDispatcher, ImmediateDomainEventDispatcher>();
        }
    }
}