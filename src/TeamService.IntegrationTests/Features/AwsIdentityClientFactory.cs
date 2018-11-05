using Amazon.Runtime;
using DFDS.TeamService.WebApi.Features.AwsRoles;
using DFDS.TeamService.WebApi.Features.AwsRoles.Infrastructure.Persistence;
using DFDS.TeamService.WebApi.Features.AwsRoles.Model;
using Moq;

namespace DFDS.TeamService.IntegrationTests.Features
{
    public class AwsIdentityClientFactory
    {
        public static AwsIdentityClient CreateFromEnviromentVariables()
        {
          var awsCredentials = new EnvironmentVariablesAWSCredentials();

            var policyRepositoryBuilder = new Mock<IPolicyRepository>();
            policyRepositoryBuilder.Setup(p => p.GetLatestAsync()).ReturnsAsync(new Policy[0]);
            var identityClient = new AwsIdentityClient(
                awsCredentials,
                policyRepositoryBuilder.Object
            );

            
            return identityClient;
        }
    }
}