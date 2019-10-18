using CorrelationId;
using Microsoft.AspNetCore.Builder;

namespace DFDS.CapabilityService.WebApi.Enablers.CorrelationId
{
    public static class Middleware
    {
        public static IApplicationBuilder UseCorrelationId(
            this IApplicationBuilder app
        )
        {
            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = "x-correlation-id",
                UpdateTraceIdentifier = true,
                IncludeInResponse = true,
                UseGuidForCorrelationId = true
            });     

            return app;
        }
    }
}