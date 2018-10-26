using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.DomainEvents
{
    public class ImmediateDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IEventSubscriberProvider _subscriberProvider;

        public ImmediateDomainEventDispatcher(IEventSubscriberProvider subscriberProvider)
        {
            _subscriberProvider = subscriberProvider;
        }

        public async Task Dispatch<TEvent>(TEvent domainEvent) where TEvent : DomainEvent
        {
            var subscribers = _subscriberProvider.GetSubscribersFor<TEvent>();

            foreach (var subscriber in subscribers)
            {
                await subscriber.Handle(domainEvent);
            }
        }
    }
}