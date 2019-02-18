using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.Tests.Helpers;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Xunit;

namespace DFDS.CapabilityService.Tests
{
    public class TestCapability
    {
        [Fact]
        public void expected_domain_event_is_raised_when_creating_a_capability()
        {
            var capability = Capability.Create("foo");

            Assert.Equal(
                expected: new[] {new CapabilityCreated(capability.Id, "foo")},
                actual: capability.DomainEvents,
                comparer: new PropertiesComparer<IDomainEvent>()
            );
        }

        [Fact]
        public void expected_domain_event_is_raised_when_membership_is_started_for_a_member()
        {
            var sut = new CapabilityBuilder().Build();
            sut.ClearDomainEvents();

            var stubMemberEmail = "foo";
            sut.StartMembershipFor(stubMemberEmail);

            Assert.Equal(
                expected: new IDomainEvent[] {new MemberJoinedCapability(sut.Id, stubMemberEmail)},
                actual: sut.DomainEvents,
                comparer: new PropertiesComparer<IDomainEvent>()
            );
        }
    }
}