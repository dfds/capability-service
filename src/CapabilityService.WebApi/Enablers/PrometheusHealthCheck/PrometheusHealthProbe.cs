using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

namespace DFDS.CapabilityService.WebApi.Enablers.PrometheusHealthCheck
{
    public static class PrometheusHealthProbe
    {
        private const string HealthCheckLabelServiceName = "service";
        private const string HealthCheckLabelStatusName = "status";

        private static readonly Gauge HealthChecksDuration;
        private static readonly Gauge HealthChecksResult;

        static PrometheusHealthProbe()
        {
            HealthChecksResult = Prometheus.Metrics.CreateGauge("healthcheck",
                "Shows health check status (status=unhealthy|degraded|healthy) 1 for triggered, otherwise 0", new GaugeConfiguration
                {
                    LabelNames = new[] {HealthCheckLabelServiceName, HealthCheckLabelStatusName},
                    SuppressInitialValue = false
                });

            HealthChecksDuration = Prometheus.Metrics.CreateGauge("healthcheck_duration_seconds",
                "Shows duration of the health check execution in seconds",
                new GaugeConfiguration
                {
                    LabelNames = new[] {HealthCheckLabelServiceName},
                    SuppressInitialValue = false
                });
        }

        public static Task WriteHealthResponseAsync(HttpContext httpContext, HealthReport healthReport)
        {
            UpdateMetrics(healthReport);

            httpContext.Response.ContentType = "text/plain";
            return httpContext.Response.WriteAsync(healthReport.Status.ToString());
        }

        private static void UpdateMetrics(HealthReport report)
        {
            foreach (var (key, value) in report.Entries)
            {
                HealthChecksResult.Labels(key, "healthy").Set(value.Status == HealthStatus.Healthy ? 1 : 0);
                HealthChecksResult.Labels(key, "unhealthy").Set(value.Status == HealthStatus.Unhealthy ? 1 : 0);
                HealthChecksResult.Labels(key, "degraded").Set(value.Status == HealthStatus.Degraded ? 1 : 0);

                HealthChecksDuration.Labels(key).Set(value.Duration.TotalSeconds);
            }
        }
    }
}