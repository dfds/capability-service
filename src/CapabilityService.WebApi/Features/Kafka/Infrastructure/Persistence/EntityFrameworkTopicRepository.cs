using System;
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
		private readonly KafkaDbContext _kafkaDbContext;
		
		private readonly CapabilityServiceDbContext _CapabilityServiceDbContext;
		private readonly Outbox _outbox;

		public EntityFrameworkTopicRepository(
			KafkaDbContext kafkaDbContext, 
			CapabilityServiceDbContext capabilityServiceDbContext, 
			Outbox outbox
		)
		{
			_kafkaDbContext = kafkaDbContext;
			_outbox = outbox;
			_CapabilityServiceDbContext = capabilityServiceDbContext;
		}
		public async Task AddAsync(Topic topic)
		{
				var daoTopic = EntityFramework.DAOs.Topic.CreateFrom(topic);
				
				await _kafkaDbContext.Topics.AddAsync(daoTopic);

				await _kafkaDbContext.SaveChangesAsync();
				try
				{
					await _outbox.QueueDomainEvents(topic);

					await _CapabilityServiceDbContext.SaveChangesAsync();
				}
				catch
				{
					_kafkaDbContext.Topics.Remove(daoTopic);

					await _kafkaDbContext.SaveChangesAsync();

					throw;
				}
		}

		public async Task<IEnumerable<Topic>> GetAllAsync()
		{
			var daoTopics = await _kafkaDbContext.Topics.ToListAsync();

			var topics = daoTopics.Select(t => Features.Kafka.Domain.Models.Topic.FromSimpleTypes(
					t.Id.ToString(),
					t.CapabilityId.ToString(),
					t.Name,
					t.Description,
					t.Partitions
				)
			);


			return topics;
		}
	}
}
