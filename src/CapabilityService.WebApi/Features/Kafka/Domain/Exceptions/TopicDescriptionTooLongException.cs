using System;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions
{
	public class TopicDescriptionTooLongException : Exception
	{
		public TopicDescriptionTooLongException() : base($"Topic description is too long. Max allowed characters is 1000.")
		{
			
		}
	}
}
