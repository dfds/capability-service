using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public static class MessagingHelper
    {
        public static string CreateMessageFrom(DomainEventEnvelope evt)
        {
            var domainEvent = JsonConvert.DeserializeObject<ExpandoObject>(evt.Data);
            var message = new
            {
                MessageId = evt.EventId,
                Type = evt.Type,
                Data = domainEvent
            };

            return JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}