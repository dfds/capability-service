using System;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;
using Xunit;

namespace DFDS.CapabilityService.Tests.Infrastructure.Messaging
{
    public class TestMessagingHelper
    {
        [Fact]
        public void returns_expected_message_format()
        {
            var e = new DomainEventEnvelope
            {
                AggregateId = "A1",
                EventId = Guid.Empty,
                Type = "foo-type",
                Data = "{\"Foo\":\"bar\"}"
            };

            var result = MessagingHelper.CreateMessageFrom(e);

            var expected = @"
{
    ""messageId"": ""00000000-0000-0000-0000-000000000000"",
    ""type"": ""foo-type"",
    ""data"": {""foo"":""bar""}
}";

            Assert.Equal(
                expected: expected.Replace(" ", "").Replace("\n", "").Replace("\r", ""),
                actual: result.Replace(" ", "").Replace("\n", "").Replace("\r", "")
            );
        }
    }
}