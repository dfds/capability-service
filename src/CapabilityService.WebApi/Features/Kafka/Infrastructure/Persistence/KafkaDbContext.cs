using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
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
				cfg.ToTable("Topic-new");
				cfg.HasKey(t => t.Id);
				cfg.OwnsOne(
					t => t.Name,
					topicName => topicName.Property(tn => tn.Name).HasColumnName("Name"));

				cfg.Ignore(x => x.DomainEvents);
			});
		}
	}
}
