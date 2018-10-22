using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DFDS.TeamService.WebApi.Features.AwsConsoleLogin
{
    public class AwsConsoleUrlBuilder : IAwsConsoleUrlBuilder, IExternalDependent
    {
        private readonly string _identityPoolId;
        private readonly string _loginProviderName;
        private readonly RegionEndpoint _regionEndpoint;
        private static readonly HttpClient HttpClient = new HttpClient();

        public AwsConsoleUrlBuilder(
            string identityPoolId,
            string loginProviderName
        )
        {
            _identityPoolId = identityPoolId;
            _loginProviderName = loginProviderName;
            
            _regionEndpoint = RegionEndpoint.GetBySystemName(identityPoolId.Split(':')[0]);

        }

        /// <summary>Creates a AWS console login uri</summary>
        /// <param name="identityToken">The token provided by the identity provider.</param>
        /// <param name="roleToAssumeArn">The Amazon Resource Name of the role the result url will give access to</param>
        public async Task<Uri> GenerateUriForConsole(
            string identityToken,
            string roleToAssumeArn
        )
        {
            var credentialsPayload = await AssumeRole(
                roleToAssumeArn,
                identityToken
            );

            var signinToken = await GetSignInToken(credentialsPayload);

            var consoleLoginUri = BuildConsoleLoginUri(signinToken);


            return consoleLoginUri;
        }


        public async Task<CredentialsPayload> AssumeRole(
            string roleToAssumeArn,
            string identityToken
        )
        {

            var cognitoAwsCredentials = new CognitoAWSCredentials(
                _identityPoolId,
                _regionEndpoint
            );

            cognitoAwsCredentials.AddLogin(
                _loginProviderName,
                identityToken
            );
            var securityTokenServiceClient = new AmazonSecurityTokenServiceClient(cognitoAwsCredentials);
            var assumedRole = await securityTokenServiceClient.AssumeRoleAsync(
                new AssumeRoleRequest
                {
                    RoleArn = roleToAssumeArn,
                    RoleSessionName = "AssumeRoleSession"
                }
            );

            var credentialsPayload = new CredentialsPayload
            {
                SessionId = assumedRole.Credentials.AccessKeyId,
                SessionKey = assumedRole.Credentials.SecretAccessKey,
                SessionToken = assumedRole.Credentials.SessionToken
            };


            return credentialsPayload;
        }


        public Uri BuildConsoleLoginUri(string signinToken)
        {
            var sb = new StringBuilder();
            sb.Append("https://signin.aws.amazon.com/federation");
            sb.Append("?Action=login");
            sb.Append("&Issuer=app.dfds.cloud");
            sb.Append("&Destination=" + WebUtility.UrlEncode("https://console.aws.amazon.com/"));
            sb.Append("&SigninToken=" + signinToken);

            var consoleLoginUri = new Uri(sb.ToString());


            return consoleLoginUri;
        }


        public async Task<string> GetSignInToken(CredentialsPayload credentialsPayload)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var session = WebUtility.UrlEncode(JsonConvert.SerializeObject(
                credentialsPayload,
                new JsonSerializerSettings
                {
                    ContractResolver = contractResolver
                }
            ));

            var uri = new Uri($"https://signin.aws.amazon.com/federation?Action=getSigninToken&Session={session}");
            
            var httpResponse = await HttpClient.GetAsync(uri);

            if (!httpResponse.IsSuccessStatusCode)
                throw new ApplicationException("Failed to create signintoken");

            var content = await httpResponse.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<Token>(content);


            return token.SigninToken;
        }

        
        public async Task<Status> GetStatusAsync()
        {
            var errorMessages = new List<string>();   

            var federationUri = new Uri("https://signin.aws.amazon.com/federation?Action=login&Issuer=https%3A%2F%2Fexample.com&Destination=https%3A%2F%2Fconsole.aws.amazon.com%2Fs&SigninToken=VCQgs5qZZt3Q6fn8Tr5EXAMPLEmLnwB7JjUc-SHwnUUWabcRdnWsi4DBn-dvCCZ85wrD0nmldUcZEXAMPLE-vXYH4Q__mleuF_W2BE5HYexbe9y4Of-kje53SsjNNecATfjIzpW1WibbnH6YcYRiBoffZBGExbEXAMPLE5aiKX4THWjQKC6gg6alHu6JFrnOJoK3dtP6I9a6hi6yPgmiOkPZMmNGmhsvVxetKzr8mx3pxhHbMEXAMPLETv1pij0rok3IyCR2YVcIjqwfWv32HU2Xlj471u3fU6uOfUComeKiqTGX974xzJOZbdmX_t_lLrhEXAMPLEDDIisSnyHGw2xaZZqudm4mo2uTDk9Pv9l5K0ZCqIgEXAMPLEcA6tgLPykEWGUyH6BdSC6166n4M4JkXIQgac7_7821YqixsNxZ6rsrpzwfnQoS14O7R0eJCCJ684EXAMPLEZRdBNnuLbUYpz2Iw3vIN0tQgOujwnwydPscM9F7foaEK3jwMkgApeb1-6L_OB12MZhuFxx55555EXAMPLEhyETEd4ZulKPdXHkgl6T9ZkIlHz2Uy1RUTUhhUxNtSQnWc5xkbBoEcXqpoSIeK7yhje9Vzhd61AEXAMPLElbWeouACEMG6-Vd3dAgFYd6i5FYoyFrZLWvm0LSG7RyYKeYN5VIzUk3YWQpyjP0RiT5KUrsUi-NEXAMPLExMOMdoODBEgKQsk-iu2ozh6r8bxwCRNhujg");
            var federationError = "Error in communicating with Amazon federation endpoint.";
            try
            {
                var getStsResponse = await HttpClient.GetAsync(federationUri);
                var content = await getStsResponse.Content.ReadAsStringAsync();
                if (getStsResponse.StatusCode != HttpStatusCode.BadRequest || !content.Contains("There was a problem with your SigninToke"))
                {
                    errorMessages.Add($"{federationError} Status code given: {getStsResponse.StatusCode}");
                }
            }
            catch
            {
                errorMessages.Add($"{federationError}");
            }
          
            
            var stsUri = new Uri("https://sts.amazonaws.com/?Version=2011-06-15&Action=AssumeRole");
            var stsError = "Error in communicating with Amazon security token service.";
            try
            {
                var getStsResponse = await HttpClient.GetAsync(stsUri);

                if (getStsResponse.StatusCode != HttpStatusCode.Forbidden)
                {
                    errorMessages.Add($"{stsError} Status code given: {getStsResponse.StatusCode}");
                }
            }
            catch
            {
                errorMessages.Add($"{stsError}");
            }


            var status = new Status();

            if (errorMessages.Any() == false)
            {
                status.IsOk = true;
                status.Message = String.Empty;

                
                return status;    
            }
            
            status.IsOk = false;
            status.Message = string.Join(Environment.NewLine, errorMessages);

            
            return status;
        }
    }


    public class CredentialsPayload
    {
        public string SessionId { get; set; }
        public string SessionKey { get; set; }
        public string SessionToken { get; set; }
    }

    public class Token
    {
        public string SigninToken { get; set; }
    }
}