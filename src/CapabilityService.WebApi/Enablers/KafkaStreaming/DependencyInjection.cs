using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dafda.Configuration;
using Dafda.Outbox;
using Dafda.Serializing;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Enablers.CorrelationId;
using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
			var capabilitiesTopicsTopicName = configuration["CAPABILITY_SERVICE_KAFKA_TOPIC_TOPICS"];

			services.AddScoped<StandardOutbox>();

			services.AddOutbox(options =>
			{
				// new messages
				options.Register<TopicRequested>(selfServiceTopic, TopicRequested.EventType, @event => @event.CapabilityRootId);

				// old messages
				options.Register<DomainEventEnvelope>(capabilitiesTopicsTopicName, "topic_created", @event => @event.AggregateId);

				options.WithOutboxEntryRepository<OutboxEntryRepository>();
				options.WithPayloadSerializer(selfServiceTopic, new DefaultPayloadSerializer());
				options.WithPayloadSerializer(capabilitiesTopicsTopicName, new LegacyContractPayloadSerializer());
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

	public class TopicRequested : IDomainEvent, IUseStandardContract
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

	public interface IUseStandardContract
	{
	}

	public class StandardOutbox
	{
		private readonly OutboxQueue _outboxQueue;
		private readonly IRequestCorrelation _requestCorrelation;
		private readonly IDomainEventRegistry _domainEventRegistry;

		public StandardOutbox(OutboxQueue outboxQueue, IRequestCorrelation requestCorrelation, IDomainEventRegistry domainEventRegistry)
		{
			_outboxQueue = outboxQueue;
			_requestCorrelation = requestCorrelation;
			_domainEventRegistry = domainEventRegistry;
		}

		public async Task Enqueue(IAggregateDomainEvents topic)
		{
			await (Task)_outboxQueue.Enqueue(PrepareDomainEvents(topic).ToList());
		}

		private IEnumerable<object> PrepareDomainEvents(IAggregateDomainEvents aggregate)
		{
			var domainEvents = aggregate.DomainEvents.ToArray();
			aggregate.ClearDomainEvents();

			foreach (var domainEvent in domainEvents)
			{
				if (domainEvent is IUseStandardContract)
				{
					yield return domainEvent;
				}
				else
				{
					yield return new DomainEventEnvelope
					{
						EventId = Guid.NewGuid(),
						AggregateId = aggregate.GetAggregateId(),
						CorrelationId = _requestCorrelation.RequestCorrelationId,
						Created = DateTime.UtcNow,
						Type = _domainEventRegistry.GetTypeNameFor(domainEvent),
						Format = "application/json",
						Data = JsonConvert.SerializeObject(domainEvent, new JsonSerializerSettings
						{
							ContractResolver = new CamelCasePropertyNamesContractResolver()
						})
					};
				}
			}
		}
	}

	public class LegacyContractPayloadSerializer : IPayloadSerializer
	{
		public string PayloadFormat => "application/json";

		public Task<string> Serialize(PayloadDescriptor payloadDescriptor)
		{
			if (!(payloadDescriptor.MessageData is DomainEventEnvelope evt))
			{
				throw new InvalidOperationException("Bad DomainEventEnvelope");
			}

			return Task.FromResult(MessagingHelper.CreateMessageFrom(evt));
		}
	}
}
