using System;
using CorrelationId;
using Serilog;

namespace DFDS.CapabilityService.WebApi.Enablers.CorrelationId
{
    public class RequestCorrelation : IRequestCorrelation
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;
        private string _overriddenCorrelationId;

        public RequestCorrelation(ICorrelationContextAccessor correlationContextAccessor) => _correlationContextAccessor = correlationContextAccessor;

        public string RequestCorrelationId
        {
            get
            {
                if (HasStoredContextCorrelationId)
                {
                    return SafeGetStoredContextCorrelationId;
                }

                return SafeGetHttpContextCorrelationId;
            }
        }

        public void OverrideCorrelationId(string correlationId)
        {
            Log.Debug("Overriding correlationId to {CorrelationId}", correlationId);
            if (!string.IsNullOrEmpty(_overriddenCorrelationId))
            {
                throw new InvalidOperationException("CorrelationId has already been overridden. CorrelationIds are locked once set in a scope.");
            }
            _overriddenCorrelationId = string.IsNullOrWhiteSpace(correlationId) ? new Guid().ToString() : correlationId;
        }
        
        private string SafeGetHttpContextCorrelationId => _correlationContextAccessor.CorrelationContext?.CorrelationId;
        private string SafeGetStoredContextCorrelationId => _overriddenCorrelationId;
        private bool HasStoredContextCorrelationId => !string.IsNullOrWhiteSpace(_overriddenCorrelationId);
    }
}