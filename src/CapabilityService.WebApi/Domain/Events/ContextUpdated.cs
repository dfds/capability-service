using System;

namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    public class ContextUpdated : IDomainEvent
    {
        public ContextUpdated(
            Guid capabilityId,
            Guid contextId, 
            string awsAccountId,
            string awsRoleArn,
            string awsRoleEmail
        )
        {
            CapabilityId = capabilityId;
            ContextId = contextId;
            AWSAccountId = awsAccountId;
            AWSRoleArn = awsRoleArn;
            AWSRoleEmail = awsRoleEmail;
        }

        public Guid CapabilityId { get;}
        public Guid ContextId { get; }
        
        public string AWSAccountId { get; }
        public string AWSRoleArn { get; }
        public string AWSRoleEmail { get; }
    }
}