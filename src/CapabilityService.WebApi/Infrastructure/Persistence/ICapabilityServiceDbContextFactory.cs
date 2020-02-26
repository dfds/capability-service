using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Persistence
{
	public interface ICapabilityServiceDbContextFactory
	{
		DbContextOptionsBuilder<CapabilityServiceDbContext> Create();
	}
}
