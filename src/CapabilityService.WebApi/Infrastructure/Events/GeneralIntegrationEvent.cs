using Newtonsoft.Json;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Events
{
    public class GeneralIntegrationEvent : IIntegrationEvent<object>
    {
        public string Version { get; private set; }
        public string EventName { get; private set; }
        [JsonProperty(PropertyName = "x-correlationId")]
        public string XCorrelationId { get; private set; }
        [JsonProperty(PropertyName = "x-sender")]
        public string XSender { get; private set; }

        public object Payload { get; private set; }

        public GeneralIntegrationEvent(
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