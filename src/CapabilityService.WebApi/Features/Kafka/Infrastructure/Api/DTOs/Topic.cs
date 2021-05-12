using System;
using System.Collections.Generic;
using System.Linq;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs
{
    public class Topic
    {    
	    public Guid Id { get; set; }
	    public string Name { get; set; }
        public string Description { get; set; }
        public Guid CapabilityId { get; set; }
        public Guid KafkaClusterId { get; set; }
        public int Partitions { get; set; }
        public Dictionary<string, object> Configurations { get; set; }

        public static Topic CreateFrom(Features.Kafka.Domain.Models.Topic topic)
        {
            return new Topic
            {
	            Id = topic.Id.Id,
                Name = topic.Name.Name,
                Description = topic.Description,
                CapabilityId = topic.CapabilityId,
                KafkaClusterId = topic.KafkaClusterId,
                Partitions = topic.Partitions,
                Configurations = topic.Configurations
            };
        }
    }
}
