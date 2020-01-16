using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using System;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions
{
	public class TopicAlreadyExistException : Exception
	{
		public TopicAlreadyExistException(TopicName name) : base($"A topic with name \"{name.Name}\" already exist.")
		{
		}
	}
}
