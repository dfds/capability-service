using System;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Events
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