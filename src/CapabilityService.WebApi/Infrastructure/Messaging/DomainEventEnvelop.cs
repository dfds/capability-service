using System;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class DomainEventEnvelop
    {
        public Guid EventId { get; set; }
        public string AggregateId { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string Data { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Sent { get; set; }
    }
}