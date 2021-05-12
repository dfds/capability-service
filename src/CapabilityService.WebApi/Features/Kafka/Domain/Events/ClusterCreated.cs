using System;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Events
{
	public class ClusterCreated : IDomainEvent
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Enabled { get; set; }
		public Guid ClusterId { get; set; }
		
		public ClusterCreated(Cluster cluster)
		{
			Name = cluster.Name;
			Description = cluster.Description;
			Enabled = cluster.Enabled;
			ClusterId = cluster.Id;
		}
	}
}
