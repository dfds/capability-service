namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public interface IRequestCorrelation
    {
        string RequestCorrelationId { get; }
        /// <summary>
        /// Override the correlation id if it needs to be set manually. Once overridden, the stored value is sticky to the thread context
        /// </summary>
        /// <param name="correlationId"></param>
        void OverrideCorrelationId(string correlationId);
    }
}