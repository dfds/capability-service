namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions
{
	public class TopicNameTooLongException : TopicNameException
	{
		public TopicNameTooLongException(string topicName, int allowedLength) : base($"Topic name '{topicName}' is {topicName.Length - allowedLength} characters longer than the allowed {allowedLength} characters.")
		{
			
		}
	}
}
