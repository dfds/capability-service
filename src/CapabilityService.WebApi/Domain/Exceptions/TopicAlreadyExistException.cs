using System;

namespace DFDS.CapabilityService.WebApi.Domain.Exceptions
{
    public class TopicAlreadyExistException : Exception
    {
        public TopicAlreadyExistException(string message) : base(message)
        {
            
        }
    }
}