using System.Collections.Generic;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class Topic : AggregateRoot<string>
    {
        private readonly List<MessageExample> _messageExamples = new List<MessageExample>();
      
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Visibility Visibility { get; private set; }
        public IEnumerable<MessageExample> MessageExamples => _messageExamples;
        
        
        public Topic(
            string name, 
            string description, 
            Visibility visibility, 
            IEnumerable<MessageExample> messageExamples
        )
        {
            Name = name;
            Description = description;
            Visibility = visibility;
            _messageExamples.AddRange(messageExamples);
        }
    }
}