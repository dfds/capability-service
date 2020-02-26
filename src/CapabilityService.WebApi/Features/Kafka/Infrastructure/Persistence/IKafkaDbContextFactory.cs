using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence
{
	public interface IKafkaDbContextFactory
	{
		DbContextOptionsBuilder<KafkaDbContext> Create();
	}
}
