using System;
using DFDS.CapabilityService.WebApi.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class Topic : AggregateRoot<Guid>
    {
        private Topic()
        {

        }

        public Topic(Guid id, string name, string description, bool isPrivate, Guid capabilityId)
        {
            Id = id;
            Name = name;
            Description = description;
            IsPrivate = isPrivate;
            CapabilityId = capabilityId;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsPrivate { get; private set; }
        public Guid CapabilityId { get; private set; }

        public static Topic Create(string name, string description, bool isPrivate, Guid capabilityId)
        {
            var topic = new Topic(
                id: Guid.NewGuid(), 
                name: name,
                description: description,
                isPrivate: isPrivate,
                capabilityId: capabilityId
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

    public class TopicAdded : IDomainEvent
    {
        public TopicAdded(string topicId, string topicName, string topicDescription, bool topicIsPrivate, string capabilityId)
        {
            TopicId = topicId;
            TopicName = topicName;
            TopicDescription = topicDescription;
            TopicIsPrivate = topicIsPrivate;
            CapabilityId = capabilityId;
        }

        public string TopicId { get; set; }
        public string TopicName { get; set; }
        public string TopicDescription { get; set; }
        public bool TopicIsPrivate { get; set; }
        public string CapabilityId { get; set; }
    }
}