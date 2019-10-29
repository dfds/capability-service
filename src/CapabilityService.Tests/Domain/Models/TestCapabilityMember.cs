using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.Tests.Helpers;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Events;
using Xunit;

namespace DFDS.CapabilityService.Tests.Domain.Models
{
    public class TestCapabilityMember
    {
        [Fact]
        public void expected_domain_event_is_raised_when_membership_is_started_for_a_member()
        {
            var sut = new CapabilityBuilder().Build();

            var stubMemberEmail = "foo";
            sut.StartMembershipFor(stubMemberEmail);

            Assert.Equal(
                expected: new IDomainEvent[] {new MemberJoinedCapability(sut.Id, stubMemberEmail)},
                actual: sut.DomainEvents,
                comparer: new PropertiesComparer<IDomainEvent>()
            );
        }

        [Fact]
        public void expected_domain_event_is_raised_when_membership_has_been_stopped_for_a_member()
        {
            var stubMemberEmail = "foo";

            var sut = new CapabilityBuilder()
                .WithMembers(stubMemberEmail)
                .Build();

            sut.StopMembershipFor(stubMemberEmail);

            Assert.Equal(
                expected: new IDomainEvent[] {new MemberLeftCapability(sut.Id, stubMemberEmail)},
                actual: sut.DomainEvents,
                comparer: new PropertiesComparer<IDomainEvent>()
            );
        }
    }
}