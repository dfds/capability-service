using System;

namespace DFDS.CapabilityService.WebApi.Domain.Exceptions
{
	public class CapabilityDoesNotExistException : Exception
	{
		public CapabilityDoesNotExistException(Guid capabilityId) : base(
			$"Capability with id {capabilityId} could not be found.")
		{
		}
	}
}
