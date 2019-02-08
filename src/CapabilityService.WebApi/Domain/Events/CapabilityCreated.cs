using System;

namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    public class CapabilityCreated : IDomainEvent
    {
        public CapabilityCreated(Guid capabilityId, string capabilityName)
        {
            CapabilityId = capabilityId;
            CapabilityName = capabilityName;
        }

        public Guid CapabilityId { get; }
        public string CapabilityName { get; }
    }
}