using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Enablers.KafkaStreaming
{
    public static class DependencyInjection
    {
        public static void AddKafkaStreamingDependencies(this IServiceCollection services)
        {
            services.AddTransient<KafkaConfiguration>();
            services.AddTransient<KafkaPublisherFactory>();
            services.AddTransient<KafkaConsumerFactory>();
            services.AddHostedService<KafkaEventPublisher>();
            services.AddHostedService<KafkaEventConsumer>();

        }
    }
}