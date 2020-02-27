using DFDS.CapabilityService.WebApi.Features.Kafka.Application;
using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence
{
	public class KafkaDbContextFactory : IKafkaDbContextFactory
	{
		private readonly string _databaseConnectionString;
		public KafkaDbContextFactory(string databaseConnectionString)
		{
			_databaseConnectionString = databaseConnectionString;
		}
		
		public DbContextOptionsBuilder<KafkaDbContext> Create()
		{
			DbContextOptionsBuilder<KafkaDbContext> dbContextOptions = new DbContextOptionsBuilder<KafkaDbContext>();
			DependencyInjection.ConfigureDbOptions(dbContextOptions, _databaseConnectionString);

			return dbContextOptions;
		}
	}
}
