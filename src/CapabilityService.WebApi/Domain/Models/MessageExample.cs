using System;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class MessageExample
    {
        public Guid Id { get; private set; }
        public string MessageType { get; private set; }
        public string Text { get; private set; }
     
        public MessageExample(
            Guid id, 
            string messageType, 
            string text
        )
        {
            Id = id;
            MessageType = messageType;
            Text = text;
        }
    }
}