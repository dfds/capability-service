using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dafda.Configuration;
using Dafda.Consuming;
using Dafda.Outbox;
using Dafda.Serializing;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Enablers.CorrelationId;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services;
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
			var confluentTopic = configuration["CAPABILITY_SERVICE_KAFKA_TOPIC_CONFLUENT"];
			var selfServiceTopic = configuration["CAPABILITY_SERVICE_KAFKA_TOPIC_SELF_SERVICE"];
			var capabilitiesTopicsTopicName = configuration["CAPABILITY_SERVICE_KAFKA_TOPIC_TOPICS"];

			services.AddConsumer(options =>
			{
				options.WithConfigurationSource(configuration);
				options.WithEnvironmentStyle("CAPABILITY_SERVICE_KAFKA");

				options.RegisterMessageHandler<TopicRequested, TopicRequestedHandler>(selfServiceTopic, TopicRequested.EventType);

				options.RegisterMessageHandler<TopicProvisioningBegun, TopicProvisionHandlers>(confluentTopic, TopicProvisioningBegun.EventType);
				options.RegisterMessageHandler<TopicProvisioned, TopicProvisionHandlers>(confluentTopic, TopicProvisioned.EventType);
				options.RegisterMessageHandler<TopicDeleted, TopicProvisionHandlers>(confluentTopic, TopicDeleted.EventType);
			});

			services.AddScoped<StandardOutbox>();

			services.AddOutbox(options =>
			{
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
		public const string EventType = "topic_requested";

		public string CapabilityRootId { get; set; }
		public string ClusterId { get; set; }
		public string TopicName { get; set; }
		public int Partitions { get; set; }
		public string Retention { get; set; }
	}

	public class TopicRequestedHandler : IMessageHandler<TopicRequested>
	{
		private readonly ICapabilityRepository _capabilityRepository;
		private readonly IClusterRepository _clusterRepository;
		private readonly ITopicDomainService _topicDomainService;

		public TopicRequestedHandler(ICapabilityRepository capabilityRepository, IClusterRepository clusterRepository, ITopicDomainService topicDomainService)
		{
			_capabilityRepository = capabilityRepository;
			_clusterRepository = clusterRepository;
			_topicDomainService = topicDomainService;
		}

		public async Task Handle(TopicRequested message, MessageHandlerContext context)
		{
			var capability = await _capabilityRepository.GetByRootId(message.CapabilityRootId);
			if (capability == null)
			{
				throw new InvalidOperationException($"Unknown capability '{message.CapabilityRootId}'");
			}

			var cluster = await _clusterRepository.GetByClusterId(message.ClusterId);
			if (cluster == null)
			{
				throw new InvalidOperationException($"Unknown cluster '{message.ClusterId}'");
			}

			var topic = Topic.Create(
				capability.Id,
				cluster.Id,
				capability.RootId,
				message.ClusterId,
				message.TopicName,
				description: "",
				message.Partitions,
				message.TopicName.StartsWith("pub.") ? "public" : "private",
				new Dictionary<string, object>
				{
					["retention.ms"] = message.Retention
				}
			);

			await _topicDomainService.CreateTopic(
				topic: topic,
				dryRun: false
			);
		}
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

	public class TopicProvisioningBegun
	{
		public const string EventType = "topic_provisioning_begun";

		public string CapabilityRootId { get; set; }
		public string ClusterId { get; set; }
		public string TopicName { get; set; }
	}

	public class TopicProvisioned
	{
		public const string EventType = "topic_provisioned";

		public string CapabilityRootId { get; set; }
		public string ClusterId { get; set; }
		public string TopicName { get; set; }
	}

	public class TopicDeleted
	{
		public const string EventType = "topic_deleted";

		public string CapabilityRootId { get; set; }
		public string ClusterId { get; set; }
		public string TopicName { get; set; }
	}
	
	public class TopicProvisionHandlers :
		IMessageHandler<TopicProvisioned>,
		IMessageHandler<TopicProvisioningBegun>,
		IMessageHandler<TopicDeleted>
	{
		private readonly ICapabilityRepository _capabilityRepository;
		private readonly IClusterRepository _clusterRepository;
		private readonly ITopicRepository _topicRepository;
		private readonly ITopicDomainService _topicDomainService;

		public TopicProvisionHandlers(ICapabilityRepository capabilityRepository, IClusterRepository clusterRepository, ITopicRepository topicRepository, ITopicDomainService topicDomainService)
		{
			_capabilityRepository = capabilityRepository;
			_clusterRepository = clusterRepository;
			_topicRepository = topicRepository;
			_topicDomainService = topicDomainService;
		}


		public  Task Handle(TopicProvisioned message, MessageHandlerContext context)
		{
			return ChangeTopicStatus(message.CapabilityRootId, message.ClusterId, message.TopicName, TopicStatus.Provisioned);
		}

		private async Task ChangeTopicStatus(string capabilityRootId, string clusterId, string topicName, TopicStatus topicStatus)
		{
			var capability = await _capabilityRepository.GetByRootId(capabilityRootId);
			if (capability == null)
			{
				throw new InvalidOperationException($"Unknown capability '{capabilityRootId}'");
			}

			var cluster = await _clusterRepository.GetByClusterId(clusterId);
			if (cluster == null)
			{
				throw new InvalidOperationException($"Unknown cluster '{clusterId}'");
			}

			await _topicRepository.GetAsync(capability.Id, cluster.Id, topicName, topicStatus);
		}

		public Task Handle(TopicProvisioningBegun message, MessageHandlerContext context)
		{
			return ChangeTopicStatus(message.CapabilityRootId, message.ClusterId, message.TopicName, TopicStatus.InProgress);		
		}
		
		public async Task Handle(TopicDeleted message, MessageHandlerContext context)
		{
			var cluster = await _clusterRepository.GetByClusterId(message.ClusterId);
			if (cluster == null)
			{
				throw new InvalidOperationException($"Unknown cluster '{message.ClusterId}'");
			}

			await _topicDomainService.DeleteTopic(message.TopicName, cluster.Id.ToString());
		}
	}
}
