using System;

namespace DFDS.CapabilityService.WebApi.Features.Topics.Domain.Exceptions
{
    public class TopicAlreadyExistException : Exception
    {
        public TopicAlreadyExistException(string message) : base(message)
        {
            
        }
    }
}