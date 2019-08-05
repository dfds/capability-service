using System;

namespace DFDS.CapabilityService.WebApi.Domain.Exceptions
{
    public class TopicDoesNotExistException : Exception
    {
        public TopicDoesNotExistException(Guid topicId) 
            : base($"Topic with id \"{topicId}\" does not exist.")
        {
            
        }
    }
}