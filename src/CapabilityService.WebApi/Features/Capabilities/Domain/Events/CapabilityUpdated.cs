
using System;
using DFDS.CapabilityService.WebApi.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Events
{
    public class CapabilityUpdated : IDomainEvent
    {
        public CapabilityUpdated(Guid capabilityId, string capabilityName, string capabilityDescription)
        {
            CapabilityId = capabilityId;
            CapabilityName = capabilityName;
            CapabilityDescription = capabilityDescription;
        }

        public Guid CapabilityId { get; }
        public string CapabilityName { get; }
        public string CapabilityDescription { get; }
    }
}