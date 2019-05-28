using System;

namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    public class ContextAddedToCapability : IDomainEvent
    {
        public ContextAddedToCapability(
            Guid capabilityId, 
            string capabilityName,
            string capabilityRootId,
            Guid contextId, 
            string contextName
        )
        {
            CapabilityId = capabilityId;
            CapabilityName = capabilityName;
            CapabilityRootId = capabilityRootId;
            ContextId = contextId;
            ContextName = contextName;
        }
        public Guid CapabilityId { get; }
        
        public string CapabilityName { get; }
        public string CapabilityRootId { get; }
        public Guid ContextId { get; }
        public string ContextName { get; }
    }
}