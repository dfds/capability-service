using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DFDS.CapabilityService.WebApi.Enablers.PrometheusHealthCheck
{
    public static class DependencyInjection
    {
        public static IHealthChecksBuilder AddPrometheusHealthCheck(
            this IServiceCollection services
        )
        {
            return services
                .AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());
        }
    }
}