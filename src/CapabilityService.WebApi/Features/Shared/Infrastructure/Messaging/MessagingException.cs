using System;

namespace DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging
{
    public class MessagingException : Exception
    {
        public MessagingException(string message) : base(message)
        {
            
        }
        public MessagingException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }

    public class MessagingHandlerNotAvailable : MessagingException
    {
        public string EventType { get; }

        public MessagingHandlerNotAvailable(string message, string eventType) : base(message)
        {
            EventType = eventType;
        }
        public MessagingHandlerNotAvailable(string message, string eventType, Exception innerException) : base(message, innerException)
        {
            EventType = eventType;
        }
    }

    public class MessagingMessageIncomprehensible : MessagingException
    {
        public MessagingMessageIncomprehensible(string message) : base(message)
        {
        }
    }
}