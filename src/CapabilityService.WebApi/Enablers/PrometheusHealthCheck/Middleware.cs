using System;
using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace DFDS.CapabilityService.WebApi.Enablers.PrometheusHealthCheck
{
    public static class Middleware
    {
        public static IApplicationBuilder UsePrometheusHealthCheck(
            this IApplicationBuilder app
        )
        {
            if (app.ApplicationServices.GetService(typeof (Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService)) == null)
                throw new InvalidOperationException("Unable to find the required services. You must call the AddHealthChecks method in ConfigureServices in the application startup code.");
            
            app.UseHealthChecks("/healthz", new HealthCheckOptions
            {
                ResponseWriter = PrometheusHealthProbe.WriteHealthResponseAsync
            });

            return app;
        }
    }
}