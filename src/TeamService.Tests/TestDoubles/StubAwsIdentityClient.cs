using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.AwsRoles;

namespace DFDS.TeamService.Tests.TestDoubles
{
    public class StubAwsIdentityClient : IAwsIdentityClient
    {
        public Task PutRoleAsync(string roleName)
        {
            return Task.CompletedTask;
        }

        public Task DeleteRole(string roleName)
        {
            return Task.CompletedTask;
        }
    }
}