using System;

namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    public class ContextAddedToCapability : IDomainEvent
    {
        public ContextAddedToCapability(Guid capabilityId, Guid contextId, string contextName)
        {
            CapabilityId = capabilityId;
            ContextId = contextId;
            ContextName = contextName;
        }
        public Guid CapabilityId { get; }
        public Guid ContextId { get; }
        public string ContextName { get; }
    }
}