using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Topic = DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models.Topic;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.RestClients
{
	public interface IKafkaJanitorRestClient
	{
		Task CreateTopic(Topic topic, Capability capability, string clusterId);
		Task RequestCredentialGeneration(Capability capability);
	}
}
