using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Kafka
{
    public class CapabilityKafkaClient : AbstractKafkaClient
    {
	    public CapabilityKafkaClient() : base(topic: "build.selfservice.events.capabilities")
	    {
	    }
    }
}
