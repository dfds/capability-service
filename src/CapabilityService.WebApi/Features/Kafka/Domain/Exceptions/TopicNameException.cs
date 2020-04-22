using System;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions
{
	public class TopicNameException : Exception
	{
		public TopicNameException(string message) : base(message)
		{
			
		}
	}
}
