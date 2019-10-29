using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;
using Newtonsoft.Json;
using Xunit;

namespace DFDS.CapabilityService.Tests.Infrastructure.Messaging
{
    public class TestEventDispatcher
    {
        [Theory]
        [InlineData("")]
        [InlineData("test-string")]
        [InlineData("{xaxa}")]
        public async Task throws_expected_exception_when_invalid_message(string generalDomainEventJson)
        {
            var sut = new EventDispatcherBuilder().Build();

            await Assert.ThrowsAsync<MessagingMessageIncomprehensible>(async () => await sut.Send(generalDomainEventJson, null));
        }
    }
}