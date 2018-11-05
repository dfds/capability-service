using System.IO;

namespace DFDS.TeamService.WebApi.Features.AwsRoles.Infrastructure.Persistence
{
    public static class PolicyDirectoryLocationFromSolution
    {
        public static PolicyDirectoryLocation Create()
        {
            var baseFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var filesFolder = Path.Combine(baseFolder, "Features/AwsRoles/Infrastructure/Persistence/Policies");
            
            
            return new PolicyDirectoryLocation(filesFolder);
        }
    }
}