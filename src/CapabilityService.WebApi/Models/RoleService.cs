using System.Threading.Tasks;

namespace DFDS.CapabilityService.WebApi.Models
{
    public class RoleService : IRoleService
    {
        private readonly IAMRoleServiceFacade _iamRoleService;
        private readonly RoleMapperServiceFacade _roleMapperService;

        public RoleService(IAMRoleServiceFacade iamRoleService, RoleMapperServiceFacade roleMapperService)
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