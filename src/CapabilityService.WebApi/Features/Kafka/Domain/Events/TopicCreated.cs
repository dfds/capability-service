using System;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Events
{
	public class TopicCreated : IDomainEvent
	{
		public TopicCreated(Topic topic)
		{
			Name = topic.Name.Name;
			Description = topic.Description;
			CapabilityId = topic.CapabilityId;
			Partitions = topic.Partitions;
		} 
		
		public string Name { get; private set; }
		public string Description { get; private set; }
		public Guid CapabilityId { get; private set; }
		public int Partitions { get; private set; }
	}
}
