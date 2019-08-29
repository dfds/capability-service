namespace DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs
{
    public class TopicInput
    {
        public string Name { get; set; }
        public string NameBusinessArea { get; set; }
        public string NameType { get; set; }
        public string NameMisc { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
    }
}