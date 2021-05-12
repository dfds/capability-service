using System;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence.EntityFramework.DAOs
{
	public class Cluster
	{
		public Cluster() {}
		
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Enabled { get; set; }
		public string ClusterId { get; set; }
	}
}
