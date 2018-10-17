using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace DFDS.TeamService.WebApi.Features.AwsRoles
{
    public class AwsIdentityClient :IDisposable, IAwsIdentityClient
    {
        private readonly AWSCredentials _awsCredentials;
        private readonly AmazonIdentityManagementServiceClient _client;

        public AwsIdentityClient(
            AWSCredentials credentials
        )
        {
            _awsCredentials = credentials;
            _client = new AmazonIdentityManagementServiceClient(
                _awsCredentials,
                RegionEndpoint.EUCentral1
            );
        }

        
        public async Task PutRoleAsync(string roleName)
        {
            await EnsureRoleExists(roleName);

            var rolePolicyRequest = new PutRolePolicyRequest
            {
                RoleName = roleName,
                PolicyName = "InlinePolicy",
                PolicyDocument = @"{
                       ""Version"": ""2012-10-17"",
                       ""Statement"": [
                           {
                               ""Effect"": ""Allow"",
                               ""Action"": ""rds:*"",
                               ""Resource"": ""*""
                           }
                       ]
                    }"
            };


            await _client.PutRolePolicyAsync(rolePolicyRequest);
        }

        
        private async Task EnsureRoleExists(string roleName)
        {
            
            var amazonSecurityTokenServiceClient = new AmazonSecurityTokenServiceClient(_awsCredentials);

            var identityResponse =
                await amazonSecurityTokenServiceClient.GetCallerIdentityAsync(new GetCallerIdentityRequest());
            var accountArn = identityResponse.Account;
            
            var createRoleRequest = new CreateRoleRequest();
            createRoleRequest.RoleName = roleName;
            createRoleRequest.AssumeRolePolicyDocument =
                @"{""Version"":""2012-10-17"",""Statement"":[{""Effect"":""Allow"",""Principal"":{""AWS"":"""+ accountArn + @"""},""Action"":""sts:AssumeRole"",""Condition"":{}}]}";


            try
            {
                await _client.CreateRoleAsync(createRoleRequest);
            }
            catch (EntityAlreadyExistsException)
            {
                // Role exists we are happy
            }
        }


        public async Task DeleteRole(string roleName)
        {
                var policiesResponse =
                    await _client.ListRolePoliciesAsync(new ListRolePoliciesRequest {RoleName = roleName});

                foreach (var policyName in policiesResponse.PolicyNames)
                {
                    await _client.DeleteRolePolicyAsync(new DeleteRolePolicyRequest
                    {
                        RoleName = roleName, 
                        PolicyName = policyName
                    });
                }

                await _client.DeleteRoleAsync(new DeleteRoleRequest {RoleName = roleName});
            }
        

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}