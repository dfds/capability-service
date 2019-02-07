using System.Linq;

namespace DFDS.CapabilityService.WebApi.Models.DTOs
{
    public static class DtoHelper
    {
        public static Capability ConvertToDto(Models.Capability capability)
        {
            return new Capability
            {
                Id = capability.Id,
                Name = capability.Name,
                Members = capability
                  .Members
                  .Select(member => new Member {Email = member.Email})
                  .ToArray()
            };
        }
    }
}