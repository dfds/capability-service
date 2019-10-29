using CorrelationId;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Enablers.CorrelationId
{
    public static class DependencyInjection
    {
        public static void AddCorrelationId(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            CorrelationIdServiceExtensions.AddCorrelationId(services);
            services.AddTransient<CorrelationIdRequestAppendHandler>();
            services.AddScoped<IRequestCorrelation, RequestCorrelation>();
        }
    }
}