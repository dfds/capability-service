using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.AwsRoles.Model;

namespace DFDS.TeamService.WebApi.Features.AwsRoles
{
    public interface IPolicyRepository
    {
        Task<IEnumerable<Policy>> GetLatestAsync();
    }
}