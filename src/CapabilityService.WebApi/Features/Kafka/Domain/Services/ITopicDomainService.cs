using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services
{
	public interface ITopicDomainService
	{
		Task CreateTopic(Topic topic, bool dryRun);
		Task<IEnumerable<Topic>> GetAllTopics();

		Task DeleteTopic(string name, string clusterId);
	}
}
