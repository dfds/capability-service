using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Integrations
{
    public class RoleService : IRoleService
    {
        private readonly IAMRoleServiceClient _iamRoleService;
        private readonly RoleMapperServiceClient _roleMapperService;

        public RoleService(IAMRoleServiceClient iamRoleService, RoleMapperServiceClient roleMapperService)
        {
            _iamRoleService = iamRoleService;
            _roleMapperService = roleMapperService;
        }
        
        public async Task CreateRoleFor(Capability capability)
        {
            var roleIdentifier = await _iamRoleService.CreateRole(capability.Name);
            await _roleMapperService.CreateRoleMapping(capability.Name, roleIdentifier);
        }
    }
}