using System;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public Guid CapabilityId { get; set; }
    }
}