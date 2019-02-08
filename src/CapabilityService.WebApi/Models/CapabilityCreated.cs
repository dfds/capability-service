using System;
using DFDS.CapabilityService.WebApi.DomainEvents;

namespace DFDS.CapabilityService.WebApi.Models
{
    public class CapabilityCreated : DomainEvent
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