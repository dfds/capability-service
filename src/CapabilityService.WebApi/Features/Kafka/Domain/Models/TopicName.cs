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
		private static int MAX_TOPIC_NAME_LENGTH = 55;

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

			if (cleanTopicName.Length < 1)
			{
				throw new TopicNameTooShortException();
			}

			var combinedString = cleanCapabilityRootIdInLowerCase + "." + cleanTopicName;
			if (combinedString.Length > MAX_TOPIC_NAME_LENGTH)
			{
				throw new TopicNameTooLongException(combinedString.ToLower(), MAX_TOPIC_NAME_LENGTH);
			}
			var charStringInLowerCase = combinedString.ToLower();

			return new TopicName(charStringInLowerCase);
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

	public class TopicNameException : Exception
	{
		public TopicNameException(string message) : base(message)
		{
			
		}
	}
	
	public class TopicNameTooLongException : TopicNameException
	{
		public TopicNameTooLongException(string topicName, int allowedLength) : base($"Topic name '{topicName}' is {topicName.Length - allowedLength} characters longer than the allowed {allowedLength} characters.")
		{
			
		}
	}

	public class TopicNameTooShortException : TopicNameException
	{
		public TopicNameTooShortException() : base("Topic name is too short.")
		{
			
		}
	}
}
