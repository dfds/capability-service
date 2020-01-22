using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories
{
	public interface ITopicRepository
	{
		Task AddAsync(Topic topic);

		Task<IEnumerable<Topic>> GetAllAsync();
	}
}
