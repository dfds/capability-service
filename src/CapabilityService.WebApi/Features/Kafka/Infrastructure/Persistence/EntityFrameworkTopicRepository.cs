using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;
using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence
{
	public class EntityFrameworkTopicRepository : ITopicRepository
	{
		private readonly ICapabilityServiceDbContextFactory _capabilityServiceDbContextFactory;
		private readonly IKafkaDbContextFactory _kafkaDbContextFactory;
		private readonly Outbox _outbox;


		public EntityFrameworkTopicRepository(
			IKafkaDbContextFactory kafkaDbContextFactory,
			ICapabilityServiceDbContextFactory capabilityServiceDbContextFactory,
			Outbox outbox)
		{
			_capabilityServiceDbContextFactory = capabilityServiceDbContextFactory;
			_kafkaDbContextFactory = kafkaDbContextFactory;
			_outbox = outbox;
		}
		public async Task AddAsync(Topic topic)
		{
			var kafkaDbContext = new KafkaDbContext(_kafkaDbContextFactory.Create().Options);
			var capabilityServiceDbContext = new CapabilityServiceDbContext(_capabilityServiceDbContextFactory.Create().Options);
			var daoTopic = EntityFramework.DAOs.Topic.CreateFrom(topic);
		
			await kafkaDbContext.Topics.AddAsync(daoTopic);

			await kafkaDbContext.SaveChangesAsync();
			try
			{
				await _outbox.QueueDomainEvents(topic);

				await capabilityServiceDbContext.SaveChangesAsync();
			}
			catch
			{
				kafkaDbContext.Topics.Remove(daoTopic);

				await kafkaDbContext.SaveChangesAsync();

				throw;
			}
			
		}

		public async Task DeleteAsync(Topic topic)
		{
			var kafkaDbContext = new KafkaDbContext(_kafkaDbContextFactory.Create().Options);
			var daoTopic = EntityFramework.DAOs.Topic.CreateFrom(topic);

			kafkaDbContext.Topics.Remove(daoTopic);
			await kafkaDbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<Topic>> GetAllAsync()
		{
			var kafkaDbContext = new KafkaDbContext(_kafkaDbContextFactory.Create().Options);

			var daoTopics = await kafkaDbContext.Topics.ToListAsync();

			var topics = daoTopics.Select(t => Features.Kafka.Domain.Models.Topic.FromSimpleTypes(
					t.Id.ToString(),
					t.CapabilityId.ToString(),
					t.Name,
					t.Description,
					t.Partitions,
					t.Created,
					t.LastModified,
					t.Configurations
				)
			);

			return topics;
		
		}
	}
}
