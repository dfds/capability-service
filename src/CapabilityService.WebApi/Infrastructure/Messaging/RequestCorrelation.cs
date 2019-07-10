using System;
using CorrelationId;
using Serilog;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class RequestCorrelation : IRequestCorrelation
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public RequestCorrelation(ICorrelationContextAccessor correlationContextAccessor) => _correlationContextAccessor = correlationContextAccessor;

        public string RequestCorrelationId => _correlationContextAccessor.CorrelationContext?.CorrelationId;
    }
}