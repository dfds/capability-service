using System;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Events
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