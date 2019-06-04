using System.Linq;
using DFDS.CapabilityService.Tests.Helpers;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Xunit;

namespace DFDS.CapabilityService.Tests.Domain.Models
{
    public class TestCapability
    {
        [Fact]
        public void expected_domain_event_is_raised_when_creating_a_capability()
        {
            var capability = Capability.Create("foo","bar");

            Assert.Equal(
                expected: new[] {new CapabilityCreated(capability.Id, "foo")},
                actual: capability.DomainEvents,
                comparer: new PropertiesComparer<IDomainEvent>()
            );
        }
        
        [Fact]
        public void rootid_is_generated_when_creating_a_capability()
        {
            var name = "foo";
            var capability = Capability.Create(name,"bar");

           Assert.StartsWith($"{name}-", capability.RootId);
   
        }

        [Fact]
        public void rootid_is_generated_when_creating_a_capability_and_is_unique()
        {
            var name = "foo";
            var capabilityOne = Capability.Create(name,"bar");
            var capabilityTwo = Capability.Create(name, "bar");

           Assert.NotEqual(capabilityOne.RootId, capabilityTwo.RootId);
        }
        
        [Fact]
        public void rootid_is_generated_from_id()
        {
            var name = "foo";
            var capability = Capability.Create(name,"bar");

            var idPartFromRootId = capability.RootId.Split('-').Last();
            
            Assert.StartsWith(idPartFromRootId, capability.Id.ToString());
        }

       


    }
}