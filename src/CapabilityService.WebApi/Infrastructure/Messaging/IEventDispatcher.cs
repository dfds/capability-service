using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Infrastructure.Events;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public interface IEventDispatcher
    {
        Task Send(string generalDomainEventJson, IServiceScope serviceScope);
        Task SendAsync(GeneralIntegrationEvent generalIntegrationEvent, IServiceScope serviceScope);
    }
    
}