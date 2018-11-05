namespace DFDS.TeamService.WebApi.Features.AwsRoles.Infrastructure.Persistence
{
    public class PolicyDirectoryLocation
    {
        private readonly string _path;

        public PolicyDirectoryLocation(string path)
        {
            _path = path;
        }
        
        public override string ToString()
        {
            return _path;
        }
        
     
        public static implicit operator PolicyDirectoryLocation (string policyDirectoryLocation)
        {
            return new PolicyDirectoryLocation(policyDirectoryLocation);
        }
        
        
        public static implicit operator string (PolicyDirectoryLocation policyDirectoryLocation)
        {
            return policyDirectoryLocation._path;
        }
    }
}