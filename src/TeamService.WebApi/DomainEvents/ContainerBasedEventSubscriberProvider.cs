using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.TeamService.WebApi.DomainEvents
{
    public class ContainerBasedEventSubscriberProvider : IEventSubscriberProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ContainerBasedEventSubscriberProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<IDomainEventSubscriber<TEvent>> GetSubscribersFor<TEvent>() where TEvent : DomainEvent
        {
            var subscribers = _serviceProvider
                .GetServices<IDomainEventSubscriber<TEvent>>()
                .ToArray();

            return subscribers;
        }
    }
}