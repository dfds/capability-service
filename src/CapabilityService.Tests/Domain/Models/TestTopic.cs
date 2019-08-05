using System.Linq;
using DFDS.CapabilityService.Tests.Builders;
using Xunit;

namespace DFDS.CapabilityService.Tests.Domain.Models
{
    public class TestTopic
    {
        [Fact]
        public void returns_expected_message_contracts_when_initialized()
        {
            var sut = new TopicBuilder().Build();
            Assert.Empty(sut.MessageContracts);
        }

        [Fact]
        public void returns_expected_message_contracts_when_having_single()
        {
            var stubMessageContract = new MessageContractBuilder().Build();

            var sut = new TopicBuilder()
                .WithMessageContracts(stubMessageContract)
                .Build();

            Assert.Equal(new[]{stubMessageContract}, sut.MessageContracts);
        }

        [Fact]
        public void returns_expected_message_contracts_when_adding_single()
        {
            var expectedType = "foo";
            var expectedDescription = "bar";
            var expectedContent = "baz";

            var sut = new TopicBuilder().Build();

            sut.AddMessageContract(expectedType, expectedDescription, expectedContent);

            var result = sut.MessageContracts.Single();

            Assert.Equal(expectedType, result.Type);
            Assert.Equal(expectedDescription, result.Description);
            Assert.Equal(expectedContent, result.Content);
        }

        [Fact]
        public void has_expected_message_contracts_after_removing_one()
        {
            var expectedType = "foo";

            var stubMessageContract = new MessageContractBuilder()
                .WithType(expectedType)
                .Build();

            var sut = new TopicBuilder()
                .WithMessageContracts(stubMessageContract)
                .Build();

            // guard assert
            Assert.NotEmpty(sut.MessageContracts);

            sut.RemoveMessageContract(expectedType);

            Assert.Empty(sut.MessageContracts);
        }
    }
}