using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence.EntityFramework.DAOs;
using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence
{
	public class KafkaDbContext : DbContext
	{
		public KafkaDbContext(DbContextOptions<KafkaDbContext> options) : base(options)
		{
		}

		public DbSet<Topic> Topics { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Topic>(cfg =>
			{
				cfg.ToTable("KafkaTopic");
				cfg.HasKey(t => t.Id);
			});
		}
	}
}
