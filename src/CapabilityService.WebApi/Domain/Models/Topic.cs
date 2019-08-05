using System;
using System.Collections.Generic;
using System.Linq;
using DFDS.CapabilityService.WebApi.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class Topic : AggregateRoot<Guid>
    {
        private readonly IList<MessageContract> _messageContracts;

        private Topic()
        {

        }

        public Topic(Guid id, string name, string description, bool isPrivate, Guid capabilityId, IEnumerable<MessageContract> messageContracts)
        {
            Id = id;
            Name = name;
            Description = description;
            IsPrivate = isPrivate;
            CapabilityId = capabilityId;
            _messageContracts = new List<MessageContract>(messageContracts);
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsPrivate { get; private set; }
        public Guid CapabilityId { get; private set; }
        public IEnumerable<MessageContract> MessageContracts => _messageContracts;

        public void AddMessageContract(string type, string description, string content)
        {
            var messageContract = new MessageContract(type, description, content);
            _messageContracts.Add(messageContract);
        }

        public void RemoveMessageContract(string type)
        {
            var contract = _messageContracts.FirstOrDefault(x => x.Type == type);
            if (contract != null)
            {
                _messageContracts.Remove(contract);
            }
        }

        public static Topic Create(string name, string description, bool isPrivate, Guid capabilityId)
        {
            var topic = new Topic(
                id: Guid.NewGuid(), 
                name: name,
                description: description,
                isPrivate: isPrivate,
                capabilityId: capabilityId,
                messageContracts: Enumerable.Empty<MessageContract>()
            );

            topic.RaiseEvent(new TopicAdded(
                topicId: topic.Id.ToString(),
                topicName: topic.Name,
                topicDescription: topic.Description,
                topicIsPrivate: topic.IsPrivate,
                capabilityId: topic.CapabilityId.ToString()
            ));

            return topic;
        }
    }
}