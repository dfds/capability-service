using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Enablers.KafkaStreaming
{
    public static class DependencyInjection
    {
        public static void AddKafkaStreaming(this IServiceCollection services)
        {
            services.AddTransient<KafkaConfiguration>();
            services.AddTransient<KafkaPublisherFactory>();
            services.AddTransient<KafkaConsumerFactory>();
            services.AddHostedService<KafkaEventPublisher>();
            services.AddHostedService<KafkaEventConsumer>();

        }
    }
}