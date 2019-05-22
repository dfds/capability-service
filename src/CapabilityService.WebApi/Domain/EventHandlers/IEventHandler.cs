using System.Threading.Tasks;

namespace DFDS.CapabilityService.WebApi.Domain.EventHandlers
{
    public interface IEventHandler<in T>
    {
        Task HandleAsync(T domainEvent);
    }
}