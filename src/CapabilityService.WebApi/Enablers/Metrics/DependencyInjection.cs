using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Enablers.Metrics
{
    public static class DependencyInjection
    {
        public static void AddMetrics(this IServiceCollection services)
        {
            services.AddHostedService<MetricHostedService>();
        }
    }
}