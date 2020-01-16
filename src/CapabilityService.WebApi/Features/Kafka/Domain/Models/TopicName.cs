using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models
{
	public class TopicName : ValueObject
	{
		public TopicName(string name)
		{
			Name = name;
		}

		public string Name { get; }
		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Name;
		}
	}
}
