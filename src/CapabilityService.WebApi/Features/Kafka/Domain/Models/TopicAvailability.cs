using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models
{
	public class TopicAvailability : ValueObject
	{
		private protected TopicAvailability(string availability)
		{
			Availability = availability;
		}

		public string Availability { get; }

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Availability;
		}

		public static TopicAvailability FromString(string availability)
		{
			var availabilityAsLowerCase = availability != null ? availability.ToLower() : "";

			if (availabilityAsLowerCase.Equals("public")) { return new TopicAvailabilityPublic(); }

			return new TopicAvailabilityPrivate();
		}
	}

	public class TopicAvailabilityPrivate : TopicAvailability
	{
		internal TopicAvailabilityPrivate() : base("private") { }
	}

	public class TopicAvailabilityPublic : TopicAvailability
	{
		internal TopicAvailabilityPublic() : base("public") { }
	}
}
