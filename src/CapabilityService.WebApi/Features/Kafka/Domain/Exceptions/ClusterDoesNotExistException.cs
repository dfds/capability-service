using System;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions
{
	public class ClusterDoesNotExistException : Exception
	{
		public ClusterDoesNotExistException(string id) : base($"No cluster with the id '{id}' was found. Try selecting a different cluster."){}
	}
}
