using System.Linq;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs
{
    public class CapabilityADSync
    {
        public string Identifier { get; set; }
        public Member[] Members { get; set; }
        
        public bool IsV1 { get; set; }
        
        public string AWSAccountId { get; set; }
        public string AWSRoleArn { get; set; }
        
        public static CapabilityADSync Create(Domain.Models.Capability capability)
        {
            return new CapabilityADSync
            {
                Identifier = capability.Name, // TODO Should vary based on conditions yet to determine
                Members = capability
                    .Members
                    .Select(member => new Member {Email = member.Email})
                    .ToArray(),
                IsV1 = true
            };
        }
    }
}