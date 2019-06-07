using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Infrastructure.Events;
using Microsoft.Extensions.Logging;

namespace DFDS.CapabilityService.WebApi.Domain.EventHandlers
{
    public class AWSContextAccountCreatedEventHandler : IEventHandler<AWSContextAccountCreatedIntegrationEvent>
    {
        private readonly ICapabilityApplicationService _capabilityApplicationService;
        private readonly ILogger<AWSContextAccountCreatedEventHandler> _logger;

        public AWSContextAccountCreatedEventHandler(ICapabilityApplicationService capabilityApplicationService
            , ILogger<AWSContextAccountCreatedEventHandler> logger)
        {
            _capabilityApplicationService = capabilityApplicationService;
            _logger = logger;
        }

        public async Task HandleAsync(AWSContextAccountCreatedIntegrationEvent domainEvent)
        {
            await _capabilityApplicationService.UpdateContext(domainEvent.Payload.CapabilityId, domainEvent.Payload.ContextId,
                domainEvent.Payload.AccountId, domainEvent.Payload.RoleArn, domainEvent.Payload.RoleEmail);
        }
    }
}