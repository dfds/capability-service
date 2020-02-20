using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.RestClients;
using Topic = DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models.Topic;

namespace DFDS.CapabilityService.Tests.TestDoubles
{
	public class StubKafkaJanitorRestClient : IKafkaJanitorRestClient
	{
		public Task CreateTopic(Topic topic, Capability capability)
		{
			return Task.CompletedTask;
		}
	}
}
