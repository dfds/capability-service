using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;
using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence
{
	public class EntityFrameworkTopicRepository : ITopicRepository
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public EntityFrameworkTopicRepository(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
		}
		public async Task AddAsync(Topic topic)
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var kafkaDbContext = scope.ServiceProvider.GetRequiredService<KafkaDbContext>();
				var capabilityServiceDbContext = scope.ServiceProvider.GetRequiredService<CapabilityServiceDbContext>();
				var outbox = scope.ServiceProvider.GetRequiredService<Outbox>();
				
				var daoTopic = EntityFramework.DAOs.Topic.CreateFrom(topic);
			
				await kafkaDbContext.Topics.AddAsync(daoTopic);

				await kafkaDbContext.SaveChangesAsync();
				try
				{
					await outbox.QueueDomainEvents(topic);

					await capabilityServiceDbContext.SaveChangesAsync();
				}
				catch
				{
					kafkaDbContext.Topics.Remove(daoTopic);

					await kafkaDbContext.SaveChangesAsync();

					throw;
				}
			}
		}

		public async Task<IEnumerable<Topic>> GetAllAsync()
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var kafkaDbContext = scope.ServiceProvider.GetRequiredService<KafkaDbContext>();

				var daoTopics = await kafkaDbContext.Topics.ToListAsync();

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
}
