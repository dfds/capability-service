using System;
using DFDS.CapabilityService.WebApi.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Events
{
    public class MemberLeftCapability : IDomainEvent
    {
        public MemberLeftCapability(Guid capabilityId, string memberEmail)
        {
            CapabilityId = capabilityId;
            MemberEmail = memberEmail;
        }

        public Guid CapabilityId { get; private set; }
        public string MemberEmail { get; private set; }
    }
}