using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models
{
	public class TopicName : ValueObject
	{
		private TopicName(string name)
		{
			Name = name;
		}

		public string Name { get; }
		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Name;
		}

		public static TopicName FromString(string fullName)
		{
			return new TopicName(fullName);
		}
			
		public static TopicName Create(
			CapabilityName capabilityName, 
			string topicName
		)
		{
			return	new TopicName(capabilityName.Name + "." + topicName);
		}
	}
}
