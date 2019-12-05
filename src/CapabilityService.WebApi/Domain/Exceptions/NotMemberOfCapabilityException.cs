using System;

namespace DFDS.CapabilityService.WebApi.Domain.Exceptions
{
    public class NotMemberOfCapabilityException : Exception
    {
        public NotMemberOfCapabilityException(Guid capabilityId, string memberEmail): base($"Capability with id {capabilityId} does not have member \"{memberEmail}\"."){}
    }
}
