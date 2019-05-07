using System;

namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    public class ContextAddedToCapability : IDomainEvent
    {
        public ContextAddedToCapability(
            Guid capabilityId, 
            string capabilityName,
            Guid contextId, 
            string contextName
        )
        {
            CapabilityId = capabilityId;
            CapabilityName = capabilityName;
            ContextId = contextId;
            ContextName = contextName;
        }
        public Guid CapabilityId { get; }
        
        public string CapabilityName { get; }
        public Guid ContextId { get; }
        public string ContextName { get; }
    }
}