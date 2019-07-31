using System;

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

        public static Topic Create(string name, string description, Guid capabilityId)
        {
            return new Topic(
                id: Guid.NewGuid(), 
                name: name,
                description: description,
                isPrivate: true,
                capabilityId: capabilityId
            );
        }
    }
}