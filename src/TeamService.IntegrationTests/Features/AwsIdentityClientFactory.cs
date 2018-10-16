using Amazon.Runtime;
using DFDS.TeamService.WebApi.Features.AwsRoles;

namespace DFDS.TeamService.IntegrationTests.Features
{
    public class AwsIdentityClientFactory
    {
        public static AwsIdentityClient CreateFromEnviromentVariables()
        {
            
            var awsCredentials = new EnvironmentVariablesAWSCredentials();
            
            
            var identityClient = new AwsIdentityClient(awsCredentials);

            
            return identityClient;
        }
    }
}