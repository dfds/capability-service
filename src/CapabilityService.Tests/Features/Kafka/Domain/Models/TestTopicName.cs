using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using Xunit;

namespace DFDS.CapabilityService.Tests.Features.Kafka.Domain.Models
{
	public class TestTopicName
	{
		[Theory]
		[InlineData("private", "FOO", "foo")]
		[InlineData("private","øæå", "oeaeaa")]
		[InlineData("private","aa_aa", "aa-aa")]
		[InlineData("private","!\"#¤%&/()=1a,", "1a")]
		[InlineData("private","1234567890 abcdefghijklmnopqrstuvwxyzæø", "1234567890-abcdefghijklmnopqrstuvwxyzaeoe")]
		[InlineData("private","我跟你讲l", "l")]
		[InlineData("private","ありがとうx", "x")]
		[InlineData("private","abcdeㅁㅊㅍ허-xㅛ히ㅐㄹ", "abcde-x")]
		[InlineData("public","aa_aa", "pub.aa-aa")]
		public void WillFormatName(string availability, string inputName, string expectedName)
		{
			var topicAvailability = TopicAvailability.FromString(availability);
			var capabilityRootId = "cap-x59a1j";
			var topicName = TopicName.Create(
				capabilityRootId, 
				inputName, 
				topicAvailability
			);

			var capabilityRootIdSection = capabilityRootId + ".";
			var resultName = topicName.Name
				.Remove(topicName.Name.IndexOf(capabilityRootIdSection),capabilityRootIdSection.Length);


			Assert.Equal(expectedName, resultName);
		}
	}
}
