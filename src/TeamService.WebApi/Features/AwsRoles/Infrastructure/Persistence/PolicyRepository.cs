using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.AwsRoles.Model;

namespace DFDS.TeamService.WebApi.Features.AwsRoles.Infrastructure.Persistence
{
    public class PolicyRepository : IPolicyRepository
    {
        public async Task<IEnumerable<Policy>> GetLatestAsync()
        {
            var policyDocument = await ReadFileAsync("rds-all-v001.json");

            var rdsPolicy = new Policy(
                "RDS-All",
                policyDocument
            );
            
            
            var policies = new[] {rdsPolicy};


            return policies;
        }

        
        public async Task<string> ReadFileAsync(string fileName)
        {
            var baseFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var fileFolder = Path.Combine(baseFolder, "Features/AwsRoles/Infrastructure/Persistence/Policies");
            var filePath = Path.Combine(fileFolder, fileName);
          
            var fileContent= await File.ReadAllTextAsync(filePath);


            return fileContent;
        }
    }
}