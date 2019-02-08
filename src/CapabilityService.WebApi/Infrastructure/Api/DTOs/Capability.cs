using System;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs
{
    public class Capability
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Member[] Members { get; set; }
    }
}