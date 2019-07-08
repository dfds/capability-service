namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public interface IRequestCorrelation
    {
        string RequestCorrelationId { get; }
    }
}