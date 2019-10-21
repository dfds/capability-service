using System;

namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    public class CapabilityDeleted : IDomainEvent
    {
        public CapabilityDeleted(
            Guid capabilityId,
            string capabilityName
        )
        {
            CapabilityId = capabilityId;
            CapabilityName = capabilityName;
        }

        public Guid CapabilityId { get; }

        public string CapabilityName { get; }
    }
}