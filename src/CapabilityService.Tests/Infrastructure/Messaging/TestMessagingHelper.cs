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
                CorrelationId = "987F693E-EF45-4518-8AFB-105B209887B1",
                Type = "foo-type",
                Data = "{\"Foo\":\"bar\"}"
            };

            var result = MessagingHelper.CreateMessageFrom(e);

            var expected = @"
{
    ""version"": ""1"",
    ""eventName"": ""foo-type"",
    ""x-correlationId"": ""987F693E-EF45-4518-8AFB-105B209887B1"",
    ""x-sender"": ""CapabilityService.WebApi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",
    ""payload"": {""foo"":""bar""}
}";

            Assert.Equal(
                expected: expected.Replace(" ", "").Replace("\n", "").Replace("\r", ""),
                actual: result.Replace(" ", "").Replace("\n", "").Replace("\r", "")
            );
        }
    }
}