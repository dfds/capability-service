using System.Collections.Generic;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
	public class CapabilityName : ValueObject
	{
		public CapabilityName(string name)
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
