using System;
using Newtonsoft.Json;

namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    public class GeneralDomainEvent : IReceivedDomainEvent<object>
    {
        public string Version { get; private set; }
        public string EventName { get; private set; }
        [JsonProperty(PropertyName = "x-correlationId")]
        public string XCorrelationId { get; private set; }
        [JsonProperty(PropertyName = "x-sender")]
        public string XSender { get; private set; }

        public object Payload { get; private set; }

        public GeneralDomainEvent(
            string version, string eventName, string xCorrelationId, string xSender, object payload)
        {
            Version = version;
            EventName = eventName;
            XCorrelationId = xCorrelationId;
            XSender = xSender;
            Payload = payload;
        }
    }
}