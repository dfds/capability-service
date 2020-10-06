using System;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions
{
	public class TopicDoesNotExistException : Exception
	{
		public TopicDoesNotExistException(TopicName name) : base($"A Topic with name {name.Name} does not exist")
		{
			
		}
	}
}
