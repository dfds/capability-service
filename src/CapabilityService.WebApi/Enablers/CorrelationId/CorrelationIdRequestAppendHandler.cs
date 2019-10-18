using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CorrelationId;

namespace DFDS.CapabilityService.WebApi.Enablers.CorrelationId
{
    public class CorrelationIdRequestAppendHandler : DelegatingHandler
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public CorrelationIdRequestAppendHandler(ICorrelationContextAccessor correlationContextAccessor)
        {
            _correlationContextAccessor = correlationContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headerName = _correlationContextAccessor.CorrelationContext.Header;
            var correlationId = _correlationContextAccessor.CorrelationContext.CorrelationId;

            if (!request.Headers.Contains(headerName))
            {
                request.Headers.Add(headerName, correlationId);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}