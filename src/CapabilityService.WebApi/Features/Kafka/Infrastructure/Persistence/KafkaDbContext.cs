using System.Collections.Generic;
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
		}
	}
}
