using System;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Infrastructure.Events;
using Newtonsoft.Json.Linq;

namespace DFDS.CapabilityService.Tests.Domain.EventHandlers
{
    public class EventBuilder
    {
        public static AWSContextAccountCreatedIntegrationEvent BuildAWSContextAccountCreatedIntegrationEvent()
        {
            dynamic payload = new JObject();
            payload.CapabilityId = Guid.NewGuid();
            payload.CapabilityName = "CapabilityNameHere";
            payload.CapabilityRootId = "blah";
            payload.ContextId = Guid.NewGuid();
            payload.ContextName = "default";

            payload.AccountId = "123456789876";
            payload.RoleArn = "iam:role::/Role/Capability";
            payload.RoleEmail = "Capability-default@dfds.com";
            
            var @event = new AWSContextAccountCreatedIntegrationEvent(new GeneralIntegrationEvent(
                "1","aws_context_account_created", Guid.NewGuid().ToString(),"", payload));
            return @event;
        }
        
    }
}