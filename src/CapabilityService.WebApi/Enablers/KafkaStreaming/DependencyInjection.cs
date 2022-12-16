using System.Collections.Generic;
using System.Threading.Tasks;
using Dafda.Configuration;
using Dafda.Outbox;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Enablers.KafkaStreaming
{
	public static class DependencyInjection
	{
		public static void AddKafkaStreaming(this IServiceCollection services)
		{
			services.AddTransient<KafkaConfiguration>();
			services.AddTransient<KafkaPublisherFactory>();
			services.AddTransient<KafkaConsumerFactory>();
			services.AddHostedService<KafkaEventPublisher>();
			services.AddHostedService<KafkaEventConsumer>();
		}

		public static void ConfigureStandardMessaging(this IServiceCollection services, IConfiguration configuration)
		{
			var selfServiceTopic = configuration["CAPABILITY_SERVICE_KAFKA_TOPIC_SELF_SERVICE"];

			services.AddOutbox(options =>
			{
				// new messages
				options.Register<TopicRequested>(selfServiceTopic, TopicRequested.EventType, @event => @event.CapabilityRootId);

				options.WithOutboxEntryRepository<OutboxEntryRepository>();
			});
		}
	}

	public class OutboxEntryRepository : IOutboxEntryRepository
	{
		private readonly KafkaDbContext _dbContext;

		public OutboxEntryRepository(KafkaDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task Add(IEnumerable<OutboxEntry> outboxEntries)
		{
			await _dbContext.OutboxEntries.AddRangeAsync(outboxEntries);
		}
	}

	public class TopicRequested : IDomainEvent
	{
		public const string EventType = "topic_requsted";

		public TopicRequested(string capabilityRootId, string clusterId, string topicName, int partitions, int retention)
		{
			CapabilityRootId = capabilityRootId;
			ClusterId = clusterId;
			TopicName = topicName;
			Partitions = partitions;
			Retention = retention;
		}

		public string CapabilityRootId { get; }
		public string ClusterId { get; }
		public string TopicName { get; }
		public int Partitions { get; }
		public int Retention { get; }
	}
}
