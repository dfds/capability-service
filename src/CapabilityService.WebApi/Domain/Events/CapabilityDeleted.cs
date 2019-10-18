using System;

namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    public class CapabilityDeleted : IDomainEvent
    {
        public CapabilityDeleted(Guid capabilityId)
        {
            CapabilityId = capabilityId;
        }

        public Guid CapabilityId { get; }
    }
}