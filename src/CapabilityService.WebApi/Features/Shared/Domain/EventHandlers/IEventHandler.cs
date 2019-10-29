using System.Threading.Tasks;

namespace DFDS.CapabilityService.WebApi.Features.Shared.Domain.EventHandlers
{
    public interface IEventHandler<in T>
    {
        Task HandleAsync(T domainEvent);
    }
}