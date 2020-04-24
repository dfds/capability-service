using System.Linq;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using Xunit;

namespace DFDS.CapabilityService.Tests.Features.Kafka.Domain.Models
{
	public class TestTopicName
	{
		[Theory]
		[InlineData("FOO", "foo")]
		[InlineData("øæå", "oeaeaa")]
		[InlineData("aa_aa", "aa-aa")]
		[InlineData("!\"#¤%&/()=1a,", "1a")]
		[InlineData("1234567890 abcdefghijklmnopqrstuvwxyzæø", "1234567890-abcdefghijklmnopqrstuvwxyzaeoe")]
		[InlineData("我跟你讲l", "l")]
		[InlineData("ありがとうx", "x")]
		[InlineData("abcdeㅁㅊㅍ허-xㅛ히ㅐㄹ", "abcde-x")]
		public void WillFormatName(string inputName, string expectedName)
		{
			var topicName = TopicName.Create("cap-x59a1j", inputName);

			var resultName = topicName.Name.Substring(topicName.Name.LastIndexOf('.') + 1);


			Assert.Equal(expectedName, resultName);
		}
	}
}
