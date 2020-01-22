using System;
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
			int partitions
		)
		{
			Id = id;
			CapabilityId = capabilityId;
			Name = name;
			Description = description;
			Partitions = partitions;
		}

		public static Topic Create(
			Guid capabilityId,
			CapabilityName capabilityName,
			string name,
			string description,
			int partitions
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
				partitions: partitions
			);

			topic.RaiseEvent(new TopicCreated(topic));


			return topic;
		}


		public TopicName Name { get; private set; }
		public string Description { get; private set; }
		public Guid CapabilityId { get; private set; }
		public int Partitions { get; private set; }

		public static Topic FromSimpleTypes(
			string id,
			string capabilityId,
			string name,
			string description,
			int partitions
		)
		{
			return new Topic(
				TopicId.FromString(id),
				Guid.Parse(capabilityId),
				TopicName.FromString(name),
				description,
				partitions
			);
		}
	}
}
