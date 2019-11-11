using System.Threading.Tasks;
using Xunit;

namespace CapabilityService.IntegrationTests.Features.Topics
{
	public class CreateTopicScenario
	{
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
			throw new System.NotImplementedException();
		}

		private async Task When_a_topic_is_created()
		{
			throw new System.NotImplementedException();
		}

		private async Task Then_the_topic_will_be_listed()
		{
			throw new System.NotImplementedException();
		}

		private void And_a_event_will_be_published()
		{
			throw new System.NotImplementedException();
		}
	}
}
