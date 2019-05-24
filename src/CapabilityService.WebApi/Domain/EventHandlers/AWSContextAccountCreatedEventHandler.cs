using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Infrastructure.Events;
using Microsoft.Extensions.Logging;

namespace DFDS.CapabilityService.WebApi.Domain.EventHandlers
{
    public class AWSContextAccountCreatedEventHandler : IEventHandler<AWSContextAccountCreatedIntegrationEvent>
    {
        private readonly ILogger<AWSContextAccountCreatedEventHandler> _logger;

        public AWSContextAccountCreatedEventHandler(ILogger<AWSContextAccountCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(AWSContextAccountCreatedIntegrationEvent domainEvent)
        {
            _logger.LogInformation($"Received AWSContextAccountCreatedIntegrationEvent");
            return Task.CompletedTask;
        }
    }
}