using System.Dynamic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging
{
    public static class MessagingHelper
    {
        public static string CreateMessageFrom(DomainEventEnvelope evt)
        {
            var domainEvent = JsonConvert.DeserializeObject<ExpandoObject>(evt.Data);
            var message = new Message(
                version: "1",
                eventName: evt.Type,
                xCorrelationId: evt.CorrelationId,
                xSender: Assembly.GetExecutingAssembly().FullName,
                payload: domainEvent);

            return JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }

    class Message
    {
        public string Version { get; private set; }
        public string EventName { get; private set; }
        [JsonProperty(PropertyName = "x-correlationId")]
        public string XCorrelationId { get; private set; }
        [JsonProperty(PropertyName = "x-sender")]
        public string XSender { get; private set; }
        public object Payload { get;  private set; }

        public Message(string version, string eventName, string xCorrelationId, string xSender, object payload)
        {
            Version = version;
            EventName = eventName;
            XCorrelationId = xCorrelationId;
            XSender = xSender;
            Payload = payload;
        }
        
    }
}