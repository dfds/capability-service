using System.Collections.Generic;
using Dafda.Outbox;
using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence.EntityFramework.DAOs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence
{
	public class KafkaDbContext : DbContext
	{
		public KafkaDbContext(DbContextOptions<KafkaDbContext> options) : base(options)
		{
		}

		public DbSet<Topic> Topics { get; set; }
		public DbSet<Cluster> Clusters { get; set; }
		public DbSet<OutboxEntry> OutboxEntries { get; set; }
		
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Topic>(cfg =>
			{
				cfg.ToTable("KafkaTopic");
				cfg.HasKey(t => t.Id);

				cfg.Property(t => t.Configurations)
					.HasConversion(
						d => JsonConvert.SerializeObject(d, Formatting.None),
						s => JsonConvert.DeserializeObject<Dictionary<string, object>>(s)
					)
					.HasMaxLength(4096);
			});

			modelBuilder.Entity<Cluster>(cfg =>
			{
				cfg.ToTable("KafkaCluster");
				cfg.HasKey(c => c.Id);
			});
			
			modelBuilder.Entity<OutboxEntry>(cfg =>
			{
				cfg.ToTable("_outbox");
				cfg.HasKey(x => x.MessageId);
				cfg.Property(x => x.MessageId).HasColumnName("Id");
				cfg.Property(x => x.Topic);
				cfg.Property(x => x.Key);
				cfg.Property(x => x.Payload);
				cfg.Property(x => x.OccurredUtc);
				cfg.Property(x => x.ProcessedUtc);
			});
		}
	}
}
