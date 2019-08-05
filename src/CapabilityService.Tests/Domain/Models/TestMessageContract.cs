using DFDS.CapabilityService.Tests.Builders;
using Xunit;

namespace DFDS.CapabilityService.Tests.Domain.Models
{
    public class TestMessageContract
    {
        [Fact]
        public void returns_expected_when_comparing_two_equal_instances()
        {
            var left = new MessageContractBuilder().Build();
            var right = new MessageContractBuilder().Build();

            Assert.Equal(left, right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_equal_instances_using_operator()
        {
            var left = new MessageContractBuilder().Build();
            var right = new MessageContractBuilder().Build();

            Assert.True(left == right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_NON_equal_instances()
        {
            var left = new MessageContractBuilder()
                .WithType("foo")
                .Build();

            var right = new MessageContractBuilder()
                .WithType("bar")
                .Build();

            Assert.NotEqual(left, right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_NON_equal_instances_using_operator()
        {
            var left = new MessageContractBuilder()
                .WithType("foo")
                .Build();

            var right = new MessageContractBuilder()
                .WithType("bar")
                .Build();

            Assert.True(left != right);
        }
    }
}