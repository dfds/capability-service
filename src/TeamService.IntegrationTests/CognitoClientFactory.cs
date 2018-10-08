using System;
using DFDS.TeamService.WebApi.Clients;

namespace DFDS.TeamService.IntegrationTests
{
    public class CognitoClientFactory
    {
        public static CognitoClient CreateFromGetEnvironmentVariables()
        {
            var accessKey = Environment.GetEnvironmentVariable("AWS_accessKey");
            var secretKey = Environment.GetEnvironmentVariable("AWS_secretKey");

            var client = new CognitoClient(
                accessKey,
                secretKey
            );


            return client;
        }
    }
}