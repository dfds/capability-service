using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence
{
	public class KafkaDbContext : DbContext
	{
		public KafkaDbContext(DbContextOptions<KafkaDbContext> options) : base(options)
		{
		}


		public DbSet<DomainEventEnvelope> DomainEvents { get; set; }

		public DbSet<Topic> Topics { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<DomainEventEnvelope>(cfg =>
			{
				cfg.HasKey(x => x.EventId);
				cfg.ToTable("DomainEvent");
			});

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
