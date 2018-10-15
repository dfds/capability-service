namespace DFDS.TeamService.WebApi
{
    /// <summary>
    /// Marker interface for all domain events
    /// </summary>
    public interface IDomainEvent
    {
        
    }

    public class DomainEventPublisher
    {
        public static void Publish<T>(T @event) where T : IDomainEvent
        {

        }
    }
}