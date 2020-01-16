using System;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models
{
	public class Topic : AggregateRoot<string>
	{
		private Topic()
		{
		}

		private Topic(
			Guid capabilityId,
			TopicName name,
			string description,
			int partitions
		)
		{
			CapabilityId = capabilityId;
			Name = name;
			Description = description;
			Partitions = partitions;

			Id = name.Name;
		}

		public static Topic Create(
			Guid capabilityId,
			CapabilityName capabilityName,
			string name,
			string description,
			int partitions
		)
		{
			var topic = new Topic(
				capabilityId: capabilityId,
				name: new TopicName(capabilityName.Name + "-" + name),
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
	}
}
