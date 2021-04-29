using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories
{
	public interface IClusterRepository
	{
		Task<IEnumerable<Cluster>> GetAllAsync();
		Task<Cluster> AddAsync(string name, string kafkaClusterId, bool enabled, string description = "");
	}
}
