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
            var isV1 = string.IsNullOrEmpty(capability.RootId);
            return new CapabilityADSync
            {
                Identifier = isV1 ? capability.Name : capability.RootId,
                IsV1 =  isV1,
                AWSAccountId = isV1 ? null : capability.Contexts?.FirstOrDefault()?.AWSAccountId,
                AWSRoleArn = isV1 ? null : capability.Contexts?.FirstOrDefault()?.AWSRoleArn,
                Members = capability
                    .Members
                    .Select(member => new Member {Email = member.Email})
                    .ToArray()
            };
        }
    }
}