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
			TopicName name,
			string description,
			int partitions,
			DateTime created,
			DateTime lastModified,
			Dictionary<string, object> configurations
		)
		{
			Id = id;
			CapabilityId = capabilityId;
			Name = name;
			Description = description;
			Partitions = partitions;
			Created = created.ToUniversalTime();
			LastModified = lastModified.ToUniversalTime();
			Configurations = configurations;
		}

		public static Topic Create(
			Guid capabilityId,
			CapabilityName capabilityName,
			string name,
			string description,
			int partitions,
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

			var topic = new Topic(
				TopicId.Create(),
				capabilityId: capabilityId,
				name: TopicName.Create(capabilityName, name),
				description: description,
				partitions: partitions,
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
		public int Partitions { get; private set; }
		public DateTime Created { get; private set; }
		public DateTime LastModified { get; private set; }
		public Dictionary<string, object> Configurations { get; private set; }


		public static Topic FromSimpleTypes(
			string id,
			string capabilityId,
			string name,
			string description,
			int partitions,
			DateTime created,
			DateTime lastModified,
			Dictionary<string, object> configurations
		)
		{
			return new Topic(
				TopicId.FromString(id),
				Guid.Parse(capabilityId),
				TopicName.FromString(name),
				description,
				partitions,
				created,
				lastModified,
				configurations
			);
		}
	}
}
