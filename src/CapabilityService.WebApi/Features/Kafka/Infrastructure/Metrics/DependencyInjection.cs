using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Metrics
{
	public static class DependencyInjection
	{
		public static void AddTopicMetrics(this IServiceCollection services)
		{
			services.AddHostedService<TopicOverseer>();
		}
	}
}
