using CorrelationId;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Enablers.CorrelationId
{
    public static class DependencyInjection
    {
        public static void AddCorrelationIdDependencies(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddCorrelationId();
            services.AddTransient<CorrelationIdRequestAppendHandler>();
            services.AddScoped<IRequestCorrelation, RequestCorrelation>();
        }
    }
}