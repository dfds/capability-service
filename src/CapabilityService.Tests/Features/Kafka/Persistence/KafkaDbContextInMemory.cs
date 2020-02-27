using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DFDS.CapabilityService.Tests.Features.Kafka.Persistence
{
	public class KafkaDbContextInMemory : IKafkaDbContextFactory
	{
		private readonly string _id;
		
		public KafkaDbContextInMemory(string id)
		{
			_id = id;
		}
		
		public DbContextOptionsBuilder<KafkaDbContext> Create()
		{
			var options = new DbContextOptionsBuilder<KafkaDbContext>();
			options.UseInMemoryDatabase(_id);
			options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
			return options;
		}
	}
}
