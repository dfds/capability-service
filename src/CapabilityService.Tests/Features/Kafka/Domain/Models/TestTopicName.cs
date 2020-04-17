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
		[InlineData("1234567890 abcdefghijklmnopqrstuvwxyzæøå.ABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ!", "1234567890-abcdefghijklmnopqrstuvwxyzaeoeaa-abcdefghijklmnopqrstuvwxyzaeoeaa")]
		public void WillFormatName(string inputName, string expectedName)
		{
			var topicName = TopicName.Create(new CapabilityName("Cap"), inputName);

			var resultName = topicName.Name.Substring(topicName.Name.LastIndexOf('.') + 1);


			Assert.Equal(expectedName, resultName);
		}

		[Fact]
		public void WillTruncateAt255Chars()
		{
			var stringOf300As = new string(Enumerable.Repeat('A', 300).ToArray());

			var topicName = TopicName.Create(new CapabilityName("Cap"), stringOf300As);

			Assert.Equal(255, topicName.Name.Length);
		}
		
		
		[Fact]
		public void WillTruncateCapabilityNameAt150Chars()
		{
			var stringOf255As = "A" + new string(Enumerable.Repeat('a', 254).ToArray());
			var topicName = TopicName.Create(new CapabilityName(stringOf255As), "name");

			Assert.Equal(150, topicName.Name.LastIndexOf('.'));
		}
	}
}
