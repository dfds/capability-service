using System;

namespace CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Api.Model
{
    public class CapabilityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RootId { get; set; }
        
        public TopicDto[] Topics { get; set; }
    }
}
