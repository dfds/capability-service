using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence
{
	public class EntityFrameworkClusterRepository : IClusterRepository
	{
		private readonly IKafkaDbContextFactory _kafkaDbContextFactory;
		private readonly Outbox _outbox;

		public EntityFrameworkClusterRepository(IKafkaDbContextFactory kafkaDbContextFactory, Outbox outbox)
		{
			_kafkaDbContextFactory = kafkaDbContextFactory;
			_outbox = outbox;
		}
		
		public async Task<IEnumerable<Cluster>> GetAllAsync()
		{
			var kafkaDbcontext = new KafkaDbContext(_kafkaDbContextFactory.Create().Options);
			var daoClusters = await kafkaDbcontext.Clusters.ToListAsync();

			var clusters = daoClusters.Select(c => Cluster.FromDao(c));

			return clusters;
		}

		public async Task<Cluster> AddAsync(string name, string kafkaClusterId, bool enabled, string description = "")
		{
			var cluster = Cluster.Create(kafkaClusterId, name, description, enabled);

			var kafkaDbcontext = new KafkaDbContext(_kafkaDbContextFactory.Create().Options);
			kafkaDbcontext.Clusters.Add(new EntityFramework.DAOs.Cluster
			{
				Id = cluster.Id,
				Name = cluster.Name,
				ClusterId = cluster.ClusterId,
				Description = cluster.Description,
				Enabled = cluster.Enabled
			});
			await kafkaDbcontext.SaveChangesAsync();
			

			return cluster;
		}
	}
}
