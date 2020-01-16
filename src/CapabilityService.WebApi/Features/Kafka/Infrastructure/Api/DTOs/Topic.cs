using System;
using System.Linq;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs
{
    public class Topic
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CapabilityId { get; set; }

        public int Partitions { get; set; }
        public static Topic CreateFrom(Features.Kafka.Domain.Models.Topic topic)
        {
            return new Topic
            {
                Name = topic.Name.Name,
                Description = topic.Description,
                CapabilityId = topic.CapabilityId,
                Partitions = topic.Partitions
            };
        }
    }
}
