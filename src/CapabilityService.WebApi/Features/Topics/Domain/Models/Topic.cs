using System;
using System.Collections.Generic;
using System.Linq;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Topics.Domain.Models
{
    public class Topic : AggregateRoot<Guid>
    {
        private readonly List<MessageContract> _messageContracts = new List<MessageContract>();

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
            _messageContracts.AddRange(messageContracts);
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public Guid CapabilityId { get; private set; }
        public IEnumerable<MessageContract> MessageContracts => _messageContracts;

        public void AddMessageContract(string type, string description, string content)
        {
            var messageContract = new MessageContract(type, description, content);
            _messageContracts.Add(messageContract);
        }

        public void UpdateMessageContract(string type, string description, string content)
        {
            var messageContract = GetMessageContract(type);

            if (messageContract == null)
            {
                return;
            }

            messageContract.Description = description;
            messageContract.Content = content;
        }

        public MessageContract GetMessageContract(string type)
        {
            return _messageContracts.SingleOrDefault(x => x.Type == type);
        }

        public bool HasMessageContract(string type)
        {
            return _messageContracts.Any(x => x.Type == type);
        }

        public void RemoveMessageContract(string type)
        {
            var contract = GetMessageContract(type);
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