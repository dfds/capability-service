using System.Collections.Generic;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs
{
	public class TopicInput
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public int Partitions { get; set; }
		public bool DryRun { get; set; }
		
		public string Availability { get; set; }
		public Dictionary<string, object> Configurations { get; set; }
	}
}
