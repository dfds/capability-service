using Amazon.Runtime;
using DFDS.TeamService.WebApi.Features.AwsRoles;
using DFDS.TeamService.WebApi.Features.AwsRoles.Infrastructure.Persistence;

namespace DFDS.TeamService.IntegrationTests.Features
{
    public class AwsIdentityClientFactory
    {
        public static AwsIdentityClient CreateFromEnviromentVariables()
        {
          var awsCredentials = new EnvironmentVariablesAWSCredentials();

            var policyRepository = new PolicyRepository();
            
            var identityClient = new AwsIdentityClient(
                awsCredentials,
                policyRepository
            );

            
            return identityClient;
        }
    }
}