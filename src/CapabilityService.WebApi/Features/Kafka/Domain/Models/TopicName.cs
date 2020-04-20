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
			string capabilityRootId,
			string topicName
		)
		{
			var cleanCapabilityRootIdInLowerCase = CleanString(capabilityRootId);
			var cleanTopicName = CleanString(topicName);

			var combinedString = cleanCapabilityRootIdInLowerCase + "." + cleanTopicName;
			var max55CharString = 55 < combinedString.Length ? combinedString.Substring(0, 55) : combinedString;
			var max55CharStringInLowerCase = max55CharString.ToLower();

			return new TopicName(max55CharStringInLowerCase);
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
