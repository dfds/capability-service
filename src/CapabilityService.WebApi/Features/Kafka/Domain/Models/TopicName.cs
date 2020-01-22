using System;
using System.Collections.Generic;
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
			var cleanCapabilityName = CleanString(capabilityName.Name);
			var max150charsCapabilityName = 150 < cleanCapabilityName.Length
				? cleanCapabilityName.Substring(0, 150)
				: cleanCapabilityName;
			var cleanTopicName = CleanString(topicName);

			var combinedString = max150charsCapabilityName + "." + cleanTopicName;
			var max255CharString = 255 < combinedString.Length ? 
				combinedString.Substring(0, 255) :
				combinedString;
			
			return new TopicName(max255CharString);
		}

		private static string CleanString(string input)
		{
			var inputLinted = input
				.Replace(' ', '-')
				.Replace('.', '-')
				.Replace('_', '-');

			var chars = inputLinted
				.Where(c =>
					Char.IsLetterOrDigit(c) ||
					c == '-'
				);


			return new String(chars.ToArray());
		}
	}
}
