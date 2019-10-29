using System;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Events;
using Newtonsoft.Json.Linq;

namespace DFDS.CapabilityService.WebApi.Features.Capabilities.Infrastructure.Events
{
    public class AWSContextAccountCreatedIntegrationEvent : IReceivedDomainEvent<AWSContextAccountCreatedData>
    {
        public AWSContextAccountCreatedIntegrationEvent(GeneralDomainEvent domainEvent)
        {
            Version = domainEvent.Version;
            EventName = domainEvent.EventName;
            XCorrelationId = domainEvent.XCorrelationId;
            XSender = domainEvent.XSender;
            Payload = (domainEvent.Payload as JObject)?.ToObject<AWSContextAccountCreatedData>();
        }

        public string Version { get; }
        public string EventName { get; }
        public string XCorrelationId { get; }
        public string XSender { get; }
        public AWSContextAccountCreatedData Payload { get; }
    }


    public class AWSContextAccountCreatedData
    {
        public Guid CapabilityId { get; }
        public string CapabilityName { get; }
        public string CapabilityRootId { get; }
        public Guid ContextId { get; }
        public string ContextName { get; }
        public string AccountId { get;  }
        public string RoleArn { get; }
        public string RoleEmail { get;  }
        
        public AWSContextAccountCreatedData(Guid capabilityId, string capabilityName, string capabilityRootId, Guid contextId, string contextName, string accountId, string roleArn, string roleEmail)
        {
            CapabilityId = capabilityId;
            CapabilityName = capabilityName;
            CapabilityRootId = capabilityRootId;
            ContextId = contextId;
            ContextName = contextName;
            AccountId = accountId;
            RoleArn = roleArn;
            RoleEmail = roleEmail;

        }
    }
}