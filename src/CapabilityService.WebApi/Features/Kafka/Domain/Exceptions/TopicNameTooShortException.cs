namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions
{
	public class TopicNameTooShortException : TopicNameException
	{
		public TopicNameTooShortException() : base("Topic name is too short.")
		{
			
		}
	}
}
