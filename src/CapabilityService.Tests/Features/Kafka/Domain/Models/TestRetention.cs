using System;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using Xunit;

namespace DFDS.CapabilityService.Tests.Features.Kafka.Domain.Models
{
	public class TestRetention
	{
		[Fact]
		public void Forever_has_correct_value()
		{
			Assert.Equal("-1", Retention.Forever);
		}

		[Fact]
		public void Default_has_correct_value()
		{
			Assert.Equal("604800000ms", Retention.Default);
		}

		[Theory]
		[InlineData(-1L, "-1")]
		[InlineData(604_800_000L, "604800000ms")]
		[InlineData(2_678_400_000L, "2678400000ms")]
		[InlineData(31_536_000_000L, "31536000000ms")]
		public void Can_parse(Int64 retentionInMs, string expected)
		{
			var sut = Retention.Parse(retentionInMs);

			Assert.Equal(expected, sut);
		}

		[Theory]
		[InlineData(-1L, "forever")]
		[InlineData(604_800_000L, "7d")]
		[InlineData(2_678_400_000L, "31d")]
		[InlineData(31_536_000_000L, "365d")]
		public void Retention_in_days(Int64 retentionInMs, string expected)
		{
			var sut = Retention.Parse(retentionInMs);

			Assert.Equal(expected, sut.Days);
		}

		[Theory]
		[InlineData(-2L)]
		[InlineData(0)]
		public void Can_handle_invalid_input(Int64 retentionInMs)
		{
			Assert.Throws<FormatException>(() => Retention.Parse(retentionInMs));
		}
	}
}
