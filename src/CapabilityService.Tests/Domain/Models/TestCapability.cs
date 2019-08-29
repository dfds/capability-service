using System;
using System.Linq;
using System.Text.RegularExpressions;
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
            var capability = Capability.Create("foo","bar", "foo");

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
            var capability = Capability.Create(name,"bar", "foo");

           Assert.StartsWith($"{name}-", capability.RootId);
   
        }

        [Fact]
        public void rootid_is_generated_when_creating_a_capability_and_is_unique()
        {
            var name = "foo";
            var capabilityOne = Capability.Create(name,"bar", "foo");
            var capabilityTwo = Capability.Create(name, "bar", "foo");

           Assert.NotEqual(capabilityOne.RootId, capabilityTwo.RootId);
        }
               
        [Fact]
        public void rootid_full_string_is_formatted_correctly()
        {
            var name = "A23456789012345678901234";
            var capability = Capability.Create(name,"bar", "foo");

            Assert.Matches("^[a-z0-9_\\-]{2,22}-[a-z]{5}$", capability.RootId);
            Assert.Equal(28, capability.RootId.Length);
        }

        [Fact]
        public void rootid_preserves_prefix_of_name()
        {
            var name = "A23456789012345678901234";
            var capability = Capability.Create(name,"bar", "foo");

            var namePrefix = capability.RootId.Split('-').First();
            
            Assert.StartsWith(namePrefix, name, StringComparison.OrdinalIgnoreCase);
        }       


    }
}