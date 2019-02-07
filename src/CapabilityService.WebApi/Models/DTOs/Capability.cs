using System;

namespace DFDS.CapabilityService.WebApi.Models.DTOs
{
    public class Capability
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Member[] Members { get; set; }
    }

    public class Member
    {
        public string Email { get; set; }
    }
}