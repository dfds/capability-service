using System.Collections.Generic;

namespace DFDS.TeamService.WebApi.DomainEvents
{
    public interface IEventSubscriberProvider
    {
        IEnumerable<IDomainEventSubscriber<TEvent>> GetSubscribersFor<TEvent>() where TEvent : DomainEvent;
    }
}