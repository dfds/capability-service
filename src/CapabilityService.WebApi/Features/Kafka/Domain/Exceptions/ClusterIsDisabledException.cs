using System;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions
{
	public class ClusterIsDisabledException : Exception
	{
		public ClusterIsDisabledException() : base($"The selected cluster does not allow new topics. Try a different cluster."){}

	}
}
