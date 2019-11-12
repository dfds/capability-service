namespace CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Kafka
{
    public class TopicKafkaClient : AbstractKafkaClient
    {
	    public TopicKafkaClient() : base(topic: "build.selfservice.events.topics")
	    {
	    }
    }
}
