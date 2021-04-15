using System;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions
{
	public class ClusterNotSelectedException : Exception
	{
		public ClusterNotSelectedException() : base($"No cluster was selected. To create a Topic, a Cluster must be specified."){}

	}
}
