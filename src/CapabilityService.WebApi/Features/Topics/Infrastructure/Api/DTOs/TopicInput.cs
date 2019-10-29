namespace DFDS.CapabilityService.WebApi.Features.Topics.Infrastructure.Api.DTOs
{
    public class TopicInput
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
    }
}