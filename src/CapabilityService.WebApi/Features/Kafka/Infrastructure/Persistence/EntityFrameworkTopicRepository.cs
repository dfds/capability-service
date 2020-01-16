using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
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
			using (TransactionScope scope = new TransactionScope())
			{
				await _kafkaDbContext.Topics.AddAsync(topic);

				await _kafkaDbContext.SaveChangesAsync();

				await _outbox.QueueDomainEvents(topic);

				await _CapabilityServiceDbContext.SaveChangesAsync();
				
				scope.Complete();
			}
		}

		public async Task<IEnumerable<Topic>> GetAllAsync()
		{
			return await _kafkaDbContext.Topics.ToListAsync();
		}
	}
}
