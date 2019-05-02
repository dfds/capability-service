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

       


    }
}