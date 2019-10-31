using System;
using System.Threading.Tasks;
using CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Api;
using Xunit;

namespace CapabilityService.IntegrationTests.Features.Capabilities
{
    public class CapabilityDeletedScenario
    {
        [Fact]
        public async Task CapabilityDeletedRecipe()
        {
            await Given_a_capability();
            When_a_capability_is_deleted();
            Then_it_wont_be_in_the_capability_list();
            And_a_event_will_be_published();
        }

        private async Task Given_a_capability()
        {
            var name = "Integration-test-" + Guid.NewGuid().ToString();
            var description = name;

            await CapabilityApiClient.Capabilities.PostAsync(name, description);
        }

        private void When_a_capability_is_deleted()
        {
            throw new System.NotImplementedException();
        }

        private void Then_it_wont_be_in_the_capability_list()
        {
            throw new System.NotImplementedException();
        }

        private void And_a_event_will_be_published()
        {
            throw new System.NotImplementedException();
        }
    }
}