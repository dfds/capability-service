using System;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Events
{
	public class TopicCreated : IDomainEvent
	{
		public TopicCreated(Topic topic, string capabilityRootId, string clusterId)
		{
			CapabilityRootId = capabilityRootId;
			ClusterId = clusterId;
			Name = topic.Name.Name;
			Description = topic.Description;
			CapabilityId = topic.CapabilityId;
			Partitions = topic.Partitions;
			Retention = topic.Configurations.GetRetentionOrDefault();
		}

		protected TopicCreated()
		{
		}
		
		public string CapabilityRootId { get; private set; }
		public string ClusterId { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public Guid CapabilityId { get; private set; }
		public int Partitions { get; private set; }
		public string Retention { get; private set; }
	}
}
