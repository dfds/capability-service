using System.Threading.Tasks;

namespace DFDS.CapabilityService.WebApi.Application.EventHandlers
{
    public interface IEventHandler<in T>
    {
        Task HandleAsync(T domainEvent);
    }
}