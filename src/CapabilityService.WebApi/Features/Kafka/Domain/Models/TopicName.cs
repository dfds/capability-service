using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
			var cleanCapabilityNameInLowerCase =
				CleanString(capabilityName.Name);

			var max150CharsCapabilityName = 150 < cleanCapabilityNameInLowerCase.Length
				? cleanCapabilityNameInLowerCase.Substring(0, 150)
				: cleanCapabilityNameInLowerCase;
			var cleanTopicName = CleanString(topicName);

			var combinedString = max150CharsCapabilityName + "." + cleanTopicName;
			var max255CharString = 255 < combinedString.Length ? combinedString.Substring(0, 255) : combinedString;
			var max255CharStringInLowerCase = max255CharString.ToLower();

			return new TopicName(max255CharStringInLowerCase);
		}

		private static string CleanString(string input)
		{
			var inputLinted = input
				.Replace(' ', '-')
				.Replace('.', '-')
				.Replace('_', '-')
				.Replace(
					oldValue: "æ",
					newValue: "ae",
					ignoreCase: true,
					culture: CultureInfo.InvariantCulture
				)
				.Replace(
					oldValue: "ø",
					newValue: "oe",
					ignoreCase: true,
					culture: CultureInfo.InvariantCulture
				)
				.Replace(
					oldValue: "å",
					newValue: "aa",
					ignoreCase: true,
					culture: CultureInfo.InvariantCulture
				);

			var chars = inputLinted
				.Where(c =>
					Char.IsLetterOrDigit(c) ||
					c == '-'
				);


			return new String(chars.ToArray());
		}
	}
}
