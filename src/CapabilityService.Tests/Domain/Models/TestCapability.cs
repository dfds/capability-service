using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Helpers;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Factories;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Xunit;

namespace DFDS.CapabilityService.Tests.Domain.Models
{
    public class TestCapability
    {
        ICapabilityFactory _capabilityFactory = new CapabilityFactory();
        [Fact]
        public async Task expected_domain_event_is_raised_when_creating_a_capability()
        {
            var capability = await _capabilityFactory.Create("Foo","bar");

            Assert.Equal(
                expected: new[] {new CapabilityCreated(capability.Id, "Foo")},
                actual: capability.DomainEvents,
                comparer: new PropertiesComparer<IDomainEvent>()
            );
        }
        
        [Fact]
        public async Task rootid_is_generated_when_creating_a_capability()
        {
            var name = "Foo";
            var capability = await _capabilityFactory.Create(name,"bar");

           Assert.StartsWith($"{name.ToLower()}-", capability.RootId);
   
        }

        [Fact]
        public async Task rootid_is_generated_when_creating_a_capability_and_is_unique()
        {
            var name = "Foo";
            var capabilityOne = await _capabilityFactory.Create(name,"bar");
            var capabilityTwo = await _capabilityFactory.Create(name, "bar");

           Assert.NotEqual(capabilityOne.RootId, capabilityTwo.RootId);
        }
        
        [Fact]
        public async Task rootid_is_generated_from_id()
        {
            var name = "Foo";
            var capability = await _capabilityFactory.Create(name,"bar");

            var idPartFromRootId = capability.RootId.Split('-').First();
            
            Assert.StartsWith(idPartFromRootId, capability.RootId);
        }

        [Theory]
        [InlineData("an-otherwise-acceptable-name")]
        [InlineData("AName!")]
        [InlineData("Aa")]
        [InlineData("A0123456789012345678901234567891A0123456789012345678901234567891A0123456789012345678901234567891A0123456789012345678901234567891A0123456789012345678901234567891A0123456789012345678901234567891A0123456789012345678901234567891A012345678901234567890123456789A")]
        public void cannot_create_capabilities_with_invalid_names(string input) {
            Assert.Throws<CapabilityValidationException>(() => _capabilityFactory.Create(input, string.Empty));
        }

        [Theory]
        [InlineData("AName")]
        [InlineData("AZ0")]
        [InlineData(
            "A0123456789012345678901234567891A0123456789012345678901234567891A0123456789012345678901234567891A0123456789012345678901234567891A0123456789012345678901234567891A0123456789012345678901234567891A0123456789012345678901234567891A012345678901234567890123456789")]
        public async Task can_create_capability_with_an_acceptable_name(string input)
        {
            await _capabilityFactory.Create(input, string.Empty);
        }

        [Theory]
        [InlineData("ADescription")]
        [InlineData("")]
        [InlineData(null)]
        public async Task can_create_capability_with_an_acceptable_description(string input)
        {
            await _capabilityFactory.Create("Foo", input);
        }
    }
}
