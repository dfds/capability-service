using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services
{
	public interface ITopicDomainService
	{
		Task CreateTopic(Topic topic, bool dryRun);
		Task<IEnumerable<Topic>> GetAllTopics();
		Task<IEnumerable<Cluster>> GetAllClusters();
		Task<Cluster> GetClusterByClusterId(string clusterId);
		Task<Cluster> GetClusterById(Guid id);

		Task DeleteTopic(string name, string clusterId);
		Task TopicProvisioned(Topic topic);
	}
}
