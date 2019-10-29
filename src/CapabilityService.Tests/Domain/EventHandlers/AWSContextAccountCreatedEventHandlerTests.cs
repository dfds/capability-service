using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi.Domain.EventHandlers;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Infrastructure.EventHandlers;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFDS.CapabilityService.Tests.Domain.EventHandlers
{
    public class AWSContextAccountCreatedEventHandlerTests
    {
        [Fact]
        public async Task application_service_is_called_on_event()
        {
            var stubAppService = new StubCapabilityApplicationService();

            var sut = new AWSContextAccountCreatedEventHandler(stubAppService,
                Dummy.Of<ILogger<AWSContextAccountCreatedEventHandler>>());

            var @event = EventBuilder.BuildAWSContextAccountCreatedIntegrationEvent();

            await sut.HandleAsync(@event);

        }
    }
}