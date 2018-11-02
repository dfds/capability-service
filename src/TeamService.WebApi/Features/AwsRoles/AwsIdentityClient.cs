using System;
using System.Collections.Generic;
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
        private readonly IPolicyRepository _policyRepository;
        public AwsIdentityClient(
            AWSCredentials credentials, 
            IPolicyRepository policyRepository
        )
        {
            _awsCredentials = credentials;
            _policyRepository = policyRepository;
            _client = new AmazonIdentityManagementServiceClient(
                _awsCredentials,
                RegionEndpoint.EUCentral1
            );
        }

        
        public async Task PutRoleAsync(string roleName)
        {
            await EnsureRoleExists(roleName);

            await PutRolePoliciesAsync(roleName);
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

        
        private async Task PutRolePoliciesAsync(string roleName)
        {
            var policies = await _policyRepository.GetLatestAsync();

            var tasks = new List<Task>();

            foreach (var policy in policies)
            {
                tasks.Add(Task.Run(async () =>
                    {
                        var rolePolicyRequest = new PutRolePolicyRequest
                        {
                            RoleName = roleName,
                            PolicyName = policy.Name,
                            PolicyDocument = policy.Document
                        };
    
                        await _client.PutRolePolicyAsync(rolePolicyRequest);
                    })
                );
            }
            
            
            Task.WaitAll(tasks.ToArray());
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