using System.Collections.Generic;
using System.Text.RegularExpressions;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
	public class CapabilityName : ValueObject
	{
		private static readonly Regex ValidNameRegex = new Regex("^[A-Z][a-zA-Z0-9\\-]{2,254}$", RegexOptions.Compiled);

		public CapabilityName(string name)
		{
			if (!ValidNameRegex.Match(name).Success)
			{
				throw new CapabilityValidationException(
					"Name must be a string of length 3 to 255. consisting of only alphanumeric ASCII characters, starting with a capital letter. Underscores and hyphens are allowed.");
			}
			
			Name = name;
		}
		public string Name { get; }
		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Name;
		}
	}
}
