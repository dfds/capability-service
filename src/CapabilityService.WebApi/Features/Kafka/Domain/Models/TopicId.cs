using System;
using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models
{
	public class TopicId : ValueObject
	{
		public Guid Id { get; }
		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Id;
		}

		private TopicId(Guid id)
		{
			Id = id;
		}

		public static TopicId FromString(string id)
		{
			var topicId = Guid.Empty;
			Guid.TryParse(id, out topicId);
			if (topicId == null) throw new TopicIdBadException(id);

			return new TopicId(topicId);
		}
		
		public static TopicId Create()
		{
			return new TopicId(Guid.NewGuid());
		}
	}
}
