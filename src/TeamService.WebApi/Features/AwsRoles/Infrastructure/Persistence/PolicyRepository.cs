using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.AwsRoles.Model;

namespace DFDS.TeamService.WebApi.Features.AwsRoles.Infrastructure.Persistence
{
    public class PolicyRepository : IPolicyRepository
    {
        public Task<IEnumerable<Policy>> GetLatestAsync()
        {
            return Task.FromResult(GetLatest());
        }
        
        
        public IEnumerable<Policy> GetLatest()
        {
            var rdsPolicy = new Policy(
                "RDS-All",
                @"{
                       ""Version"": ""2012-10-17"",
                       ""Statement"": [
                           {
                               ""Effect"": ""Allow"",
                               ""Action"": ""rds:*"",
                               ""Resource"": ""*""
                           }
                       ]
                    }"
            );

            var policies = new [] {rdsPolicy};

            
            return policies;
        }
    }
}