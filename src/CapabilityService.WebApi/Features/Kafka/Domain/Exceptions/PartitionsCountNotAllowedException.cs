using System;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions
{
	public class PartitionsCountNotAllowedException : Exception
	{
		public PartitionsCountNotAllowedException(int lowerAllowedCount, int upperAllowedCount, int partitionCount) :
			base($"The partition count: '{partitionCount}' is outside the allowed boundary of {lowerAllowedCount} to {upperAllowedCount}"){}
	}
}
