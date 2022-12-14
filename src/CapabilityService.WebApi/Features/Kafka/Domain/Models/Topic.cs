using System;
using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models
{
	public class Topic : AggregateRoot<TopicId>
	{
		private Topic(
			TopicId id,
			Guid capabilityId,
			Guid kafkaClusterId,
			TopicName name,
			string description,
			int partitions,
			TopicStatus status,
			DateTime created,
			DateTime lastModified,
			Dictionary<string, object> configurations
		)
		{
			Id = id;
			CapabilityId = capabilityId;
			KafkaClusterId = kafkaClusterId;
			Name = name;
			Description = description;
			Partitions = partitions;
			Status = status;
			Created = created.ToUniversalTime();
			LastModified = lastModified.ToUniversalTime();
			Configurations = configurations;
		}

		public static Topic Create(
			Guid capabilityId,
			Guid kafkaClusterId,
			string capabilityRootId,
			string name,
			string description,
			int partitions,
			Dictionary<string, object> configurations
		)
		{
			return Create(
				capabilityId,
				kafkaClusterId,
				capabilityRootId,
				name,
				description,
				partitions,
				"private",
				configurations
			);
		}

		public static Topic Create(
			Guid capabilityId,
			Guid kafkaClusterId,
			string capabilityRootId,
			string name,
			string description,
			int partitions,
			string availability,
			Dictionary<string, object> configurations
		)
		{
			const int lowerAllowedPartitions = 1;
			const int upperAllowedPartitions = 12;
			if (partitions < lowerAllowedPartitions || upperAllowedPartitions < partitions)
			{
				throw new PartitionsCountNotAllowedException(
					lowerAllowedPartitions,
					upperAllowedPartitions,
					partitions
				);
			}

			if (description.Length > 1000)
			{
				throw new TopicDescriptionTooLongException();
			}

			var topicAvailability = TopicAvailability.FromString(availability);
			var topicName = TopicName.Create(
				capabilityRootId, 
				name, 
				topicAvailability
			);
			
			var topic = new Topic(
				TopicId.Create(),
				capabilityId: capabilityId,
				kafkaClusterId: kafkaClusterId,
				name: topicName,
				description: description,
				partitions: partitions,
				status: TopicStatus.Requested,
				created: DateTime.UtcNow,
				lastModified: DateTime.UtcNow,
				configurations: configurations
			);

			topic.RaiseEvent(new TopicCreated(topic));


			return topic;
		}


		public TopicName Name { get; private set; }
		public string Description { get; private set; }
		public Guid CapabilityId { get; private set; }
		public Guid KafkaClusterId { get; private set; }
		public int Partitions { get; private set; }
		public TopicStatus Status { get; private set; }
		public DateTime Created { get; private set; }
		public DateTime LastModified { get; private set; }
		public Dictionary<string, object> Configurations { get; private set; }


		public static Topic FromSimpleTypes(
			string id,
			string capabilityId,
			string kafkaClusterId,
			string name,
			string description,
			int partitions,
			string status,
			DateTime created,
			DateTime lastModified,
			Dictionary<string, object> configurations
		)
		{
			return new Topic(
				TopicId.FromString(id),
				Guid.Parse(capabilityId),
				Guid.Parse(kafkaClusterId),
				TopicName.FromString(name),
				description,
				partitions,
				Enum.Parse<TopicStatus>(status),
				created,
				lastModified,
				configurations
			);
		}
	}

	public enum TopicStatus
	{
		Requested,
		InProgress,
		Provisioned
	}
}
