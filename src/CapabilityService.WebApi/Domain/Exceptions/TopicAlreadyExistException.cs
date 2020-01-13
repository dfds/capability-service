using System;

namespace DFDS.CapabilityService.WebApi.Domain.Exceptions
{
    public class TopicAlreadyExistException : Exception
    {
        public TopicAlreadyExistException(string name) : base($"A topic with name \"{name}\" already exist.")
        {
            
        }
    }
}
