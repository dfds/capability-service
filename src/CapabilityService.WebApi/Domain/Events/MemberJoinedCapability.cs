using System;

namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    public class MemberJoinedCapability : IDomainEvent
    {
        public MemberJoinedCapability(Guid capabilityId, string memberEmail)
        {
            CapabilityId = capabilityId;
            MemberEmail = memberEmail;
        }

        public Guid CapabilityId { get; private set; }
        public string MemberEmail { get; private set; }
    }
}