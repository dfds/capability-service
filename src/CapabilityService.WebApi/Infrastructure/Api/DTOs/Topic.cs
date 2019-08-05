using System;
using System.Linq;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public Guid CapabilityId { get; set; }
        public MessageContract[] MessageContracts { get; set; }

        public static Topic CreateFrom(Domain.Models.Topic topic)
        {
            return new Topic
            {
                Id = topic.Id,
                Name = topic.Name,
                Description = topic.Description,
                IsPrivate = topic.IsPrivate,
                CapabilityId = topic.CapabilityId,
                MessageContracts = topic.MessageContracts
                    .Select(MessageContract.CreateFrom)
                    .ToArray()
            };
        }
    }
}