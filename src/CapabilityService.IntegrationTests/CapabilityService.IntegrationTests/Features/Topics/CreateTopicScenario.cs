using System;
using System.Threading.Tasks;
using CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Api;
using CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Api.Model;
using CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Kafka;
using CapabilityService.IntegrationTests.Features.Topics.Infrastructure.Api;
using Xunit;

namespace CapabilityService.IntegrationTests.Features.Topics
{
	public class CreateTopicScenario
	{
		private CapabilityDto _capabilityDto;
		[Fact]
		public async Task CreateTopicRecipe()
		{
			await Given_a_capability();
			await When_a_topic_is_created();
			await Then_the_topic_will_be_listed();
				  And_a_event_will_be_published();
		}

		private async Task Given_a_capability()
		{
			var name = "Integration-test-create-topic" + Guid.NewGuid().ToString().Substring(0, 8);
			var description = name;

			_capabilityDto = await CapabilityApiClient.Capabilities.PostAsync(name, description);
			await CapabilityApiClient.Capabilities.GetAsync(_capabilityDto.Id);
		}

		private async Task When_a_topic_is_created()
		{
			var name = "Integration-test-create-topic" + Guid.NewGuid().ToString().Substring(0, 8);

			await TopicApiClient.CreateTopic(
				_capabilityDto.Id,
				name,
				"A topic created to prove we can create a topic"
			);
		}

		private async Task Then_the_topic_will_be_listed()
		{
			throw new System.NotImplementedException();
		}

		private void And_a_event_will_be_published()
		{
			throw new System.NotImplementedException();
			var capabilityKafkaClient = new CapabilityKafkaClient();
			capabilityKafkaClient.GetUntil(
				WeFoundTheEventWeWant, 
				TimeSpan.FromSeconds(20)
			);
		}
		
		public bool WeFoundTheEventWeWant(dynamic @event)
		{
			if(@event == null) {return false;}
            
			if(
				@event.eventName !="topic_added" || 
				@event.payload.capabilityId != _capabilityDto.Id.ToString()
			) {return false;}


			return true;
		}
	}
}
