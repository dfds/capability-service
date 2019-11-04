using System;
using System.Threading.Tasks;
using CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Api;
using CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Api.Model;
using CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Kafka;
using Xunit;

namespace CapabilityService.IntegrationTests.Features.Capabilities
{
    public class CapabilityDeletedScenario
    {
        private CapabilityDto _capabilityDto;

        [Fact]
        public async Task CapabilityDeletedRecipe()
        {
            await Given_a_capability();
            await When_a_capability_is_deleted();
            await Then_it_wont_be_in_the_capability_list();
            And_a_event_will_be_published();
        }

        private async Task Given_a_capability()
        {
            var name = "Integration-test-" + Guid.NewGuid().ToString().Substring(0, 8);
            var description = name;

            _capabilityDto = await CapabilityApiClient.Capabilities.PostAsync(name, description);
            await CapabilityApiClient.Capabilities.GetAsync(_capabilityDto.Id);
        }

        private async Task When_a_capability_is_deleted()
        {
            await CapabilityApiClient.Capabilities.DeleteAsync(_capabilityDto.Id);
        }

        private async Task Then_it_wont_be_in_the_capability_list()
        {
            var capabilities = await CapabilityApiClient.Capabilities.GetAsync();

            Assert.All(capabilities.Items, dto => Assert.False(dto.Id.Equals(_capabilityDto.Id)));
        }

        private void And_a_event_will_be_published()
        {
            var capabilityKafkaClient = new CapabilityKafkaClient();
          var events =  capabilityKafkaClient.GetUntilEventName(
                WefoundTheEventWeWant, 
                TimeSpan.FromSeconds(20)
            );
        }

        public bool WefoundTheEventWeWant(dynamic @event)
        {
            if(@event == null) {return false;}
            
            if(
                @event.eventName !="capability_deleted" || 
                @event.payload.capabilityId != _capabilityDto.Id.ToString()
            ) {return false;}


            return true;
        }
    }
}