using System;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class MessagingException : Exception
    {
        public MessagingException(string message) : base(message)
        {
            
        }
    }
}