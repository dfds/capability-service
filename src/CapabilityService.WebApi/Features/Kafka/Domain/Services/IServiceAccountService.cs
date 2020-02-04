using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services
{
	public interface IServiceAccountService
	{
		Task EnsureServiceAccountAvailability(Capability capability, Topic topic);
	}
}
