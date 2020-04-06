using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Metrics
{
	public static class DependencyInjection
	{
		public static void AddTopicMetrics(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddOptions<TopicOverseerOptions>();
			services.Configure<TopicOverseerOptions>(configuration);
			services.AddHostedService<TopicOverseer>();
		}
	}
}
