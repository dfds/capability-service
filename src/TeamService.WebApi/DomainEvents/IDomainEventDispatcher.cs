using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.DomainEvents
{
    public interface IDomainEventDispatcher
    {
        Task Dispatch<TEvent>(TEvent domainEvent) where TEvent : DomainEvent;
    }
}