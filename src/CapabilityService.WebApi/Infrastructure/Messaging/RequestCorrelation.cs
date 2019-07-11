using System;
using System.Threading;
using CorrelationId;
using Serilog;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class RequestCorrelation : IRequestCorrelation
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;
        private static readonly AsyncLocal<string> OverriddenCorrelationId = new AsyncLocal<string>();

        public RequestCorrelation(ICorrelationContextAccessor correlationContextAccessor) => _correlationContextAccessor = correlationContextAccessor;

        public string RequestCorrelationId
        {
            get
            {
                return string.IsNullOrEmpty(OverriddenCorrelationId.Value)
                    ? _correlationContextAccessor.CorrelationContext?.CorrelationId
                    : OverriddenCorrelationId.Value;
            }
        }

        public void OverrideCorrelationId(string correlationId)
        {
            Log.Information("Overriding correlationId in AsyncLocal to be {CorrelationId}", correlationId);
            if (!String.IsNullOrEmpty(OverriddenCorrelationId.Value))
            {
                throw new InvalidOperationException("CorrelationId has already been overridden. CorrelationIds are locked once set in a scope.");
            }
            OverriddenCorrelationId.Value = string.IsNullOrWhiteSpace(correlationId) ? new Guid().ToString() : correlationId;
        }
    }
}