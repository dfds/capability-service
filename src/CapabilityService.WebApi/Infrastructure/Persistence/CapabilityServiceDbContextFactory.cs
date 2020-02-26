using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Persistence
{
	public class CapabilityServiceDbContextFactory : ICapabilityServiceDbContextFactory
	{
		private readonly string _databaseConnectionString;

		public CapabilityServiceDbContextFactory(string databaseConnectionString)
		{
			_databaseConnectionString = databaseConnectionString;
		}

		public DbContextOptionsBuilder<CapabilityServiceDbContext> Create()
		{
			DbContextOptionsBuilder<CapabilityServiceDbContext> dbContextOptions = new DbContextOptionsBuilder<CapabilityServiceDbContext>();
			dbContextOptions.UseNpgsql(_databaseConnectionString);
			return dbContextOptions;
		}
	}
}
