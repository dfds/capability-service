using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public interface IEventDispatcher
    {
        Task Send(string generalDomainEventJson, IServiceScope serviceScope);
        Task SendAsync(GeneralDomainEvent generalDomainEvent, IServiceScope serviceScope);
    }
    
}