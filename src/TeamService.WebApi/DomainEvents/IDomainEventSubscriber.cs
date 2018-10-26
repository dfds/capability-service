using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.DomainEvents
{
    public interface IDomainEventSubscriber<TEvent> where TEvent : DomainEvent
    {
        Task Handle(TEvent domainEvent);
    }
}