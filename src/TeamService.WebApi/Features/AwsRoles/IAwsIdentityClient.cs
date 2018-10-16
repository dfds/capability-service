using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.Features.AwsRoles
{
    public interface IAwsIdentityClient
    {
        Task PutRoleAsync(string roleName);
        Task DeleteRole(string roleName);
    }
}