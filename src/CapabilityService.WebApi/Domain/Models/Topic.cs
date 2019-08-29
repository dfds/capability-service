using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DFDS.CapabilityService.WebApi.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class Topic : AggregateRoot<Guid>
    {
        private readonly List<MessageContract> _messageContracts = new List<MessageContract>();

        private Topic()
        {

        }

        public Topic(Guid id, string name, string description, string nameBusinessArea, string nameType, string nameMisc, bool isPrivate, Guid capabilityId, IEnumerable<MessageContract> messageContracts)
        {
            Id = id;
            Name = name;
            NameBusinessArea = nameBusinessArea;
            NameType = nameType;
            NameMisc = nameMisc;
            Description = description;
            IsPrivate = isPrivate;
            CapabilityId = capabilityId;
            _messageContracts.AddRange(messageContracts);
        }

        public string Name { get; set; }
        public string NameBusinessArea { get; set; }
        public string NameType { get; set; }
        public string NameMisc { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public Guid CapabilityId { get; private set; }
        public IEnumerable<MessageContract> MessageContracts => _messageContracts;

        public string GetName(Capability capability) // This doesn't feel quite right, there should/is probably be a better way to do this without requiring the Capability as parameter.
        {
            var nameBuilder = new StringBuilder();
            var name = "";
            nameBuilder.Append(NameBusinessArea + "." + capability.TopicCommonPrefix);
            name = NameBusinessArea + "." + capability.TopicCommonPrefix;
            if (NameType.Length > 0)
            {
                nameBuilder.Append("." + NameType);
                name = name + "." + NameType;
            }

            if (NameMisc.Length > 0)
            {
                nameBuilder.Append("." + NameMisc);
                name = name + "." + NameMisc;
            }

            return name;
        }

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

        public static Topic Create(string name, string nameBusinessArea, string nameType, string nameMisc, string description, bool isPrivate, Guid capabilityId)
        {
            var topic = new Topic(
                id: Guid.NewGuid(), 
                name: name,
                nameBusinessArea: nameBusinessArea,
                nameType: nameType,
                nameMisc: nameMisc,
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