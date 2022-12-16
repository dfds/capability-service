using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Enablers.KafkaStreaming;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence
{
	public class EntityFrameworkTopicRepository : ITopicRepository
	{
		private readonly IKafkaDbContextFactory _kafkaDbContextFactory;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public EntityFrameworkTopicRepository(IKafkaDbContextFactory kafkaDbContextFactory, IServiceScopeFactory serviceScopeFactory)
		{
			_kafkaDbContextFactory = kafkaDbContextFactory;
			_serviceScopeFactory = serviceScopeFactory;
		}

		public async Task AddAsync(Topic topic)
		{
			using var scope = _serviceScopeFactory.CreateScope();

			var kafkaDbContext = scope.ServiceProvider.GetRequiredService<KafkaDbContext>();
			await using var transaction = await kafkaDbContext.Database.BeginTransactionAsync();

			await kafkaDbContext.Topics.AddAsync(EntityFramework.DAOs.Topic.CreateFrom(topic));

			var outbox = scope.ServiceProvider.GetRequiredService<StandardOutbox>();
			await outbox.Enqueue(topic);

			await kafkaDbContext.SaveChangesAsync();
			await transaction.CommitAsync();
		}

		public async Task DeleteAsync(Topic topic)
		{
			var kafkaDbContext = new KafkaDbContext(_kafkaDbContextFactory.Create().Options);
			var daoTopic = EntityFramework.DAOs.Topic.CreateFrom(topic);

			kafkaDbContext.Topics.Remove(daoTopic);
			await kafkaDbContext.SaveChangesAsync();
		}

		public async Task<Topic> GetAsync(Guid capabilityId, Guid kafkaClusterId, string topicName)
		{
			var kafkaDbContext = new KafkaDbContext(_kafkaDbContextFactory.Create().Options);

			var t = await kafkaDbContext.Topics
				.Where(x => x.CapabilityId==capabilityId)
				.Where(x => x.KafkaClusterId==kafkaClusterId)
				.Where(x => x.Name==topicName)
				.SingleOrDefaultAsync();

			if (t == null)
			{
				return null;
			}

			return Topic.FromSimpleTypes(
				t.Id.ToString(),
				t.CapabilityId.ToString(),
				t.KafkaClusterId.ToString(),
				t.Name,
				t.Description,
				t.Partitions,
				t.Status,
				t.Created,
				t.LastModified,
				t.Configurations
			);
		}

		public async Task<IEnumerable<Topic>> GetAllAsync()
		{
			var kafkaDbContext = new KafkaDbContext(_kafkaDbContextFactory.Create().Options);

			var daoTopics = await kafkaDbContext.Topics.ToListAsync();

			var topics = daoTopics.Select(t => Features.Kafka.Domain.Models.Topic.FromSimpleTypes(
					t.Id.ToString(),
					t.CapabilityId.ToString(),
					t.KafkaClusterId.ToString(),
					t.Name,
					t.Description,
					t.Partitions,
					t.Status,
					t.Created,
					t.LastModified,
					t.Configurations
				)
			);

			return topics;
		
		}
	}
}
