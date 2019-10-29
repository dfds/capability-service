using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging
{
    public interface IEventDispatcher
    {
        Task Send(string generalDomainEventJson, IServiceScope serviceScope);
        Task SendAsync(GeneralDomainEvent generalDomainEvent, IServiceScope serviceScope);
    }
    
}