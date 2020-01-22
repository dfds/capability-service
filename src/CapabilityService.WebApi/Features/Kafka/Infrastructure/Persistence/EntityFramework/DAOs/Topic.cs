using System;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence.EntityFramework.DAOs
{
	public class Topic
	{
		public Topic(){}
		
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public Guid CapabilityId { get; set; }

		public int Partitions { get; set; }
		
		public static Topic CreateFrom(Features.Kafka.Domain.Models.Topic topic)
		{
			return new Topic
			{
				Id = topic.Id.Id,
				Name = topic.Name.Name,
				Description = topic.Description,
				CapabilityId = topic.CapabilityId,
				Partitions = topic.Partitions
			};
		}
	}
}
