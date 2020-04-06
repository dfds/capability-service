using Microsoft.Extensions.Configuration;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Metrics
{
	public class TopicOverseerOptions
	{
		public string CAPABILITYSERVICE_FEATURES_TOPIC_METRICS_EVERY_X_SECONDS { get; set; }
		
		public TopicOverseerOptions() {}

		public TopicOverseerOptions(IConfiguration conf)
		{
			CAPABILITYSERVICE_FEATURES_TOPIC_METRICS_EVERY_X_SECONDS =
				conf["CAPABILITYSERVICE_FEATURES_TOPIC_METRICS_EVERY_X_SECONDS"];
		}
	}
}
