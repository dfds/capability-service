using System;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Models;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class ContextBuilder
    {
        private Guid _id = new Guid("F26BE294-B9FD-4F97-833D-927F60662500");
        private string _name = "default";
        private string _awsAccountId = "222222222222";
        private string _awsRoleArn = "arn:aws:iam::528563840976:role/aws-elasticbeanstalk-ec2-role";
        private string _awsRoleEmail = "dfdscpapa@dfds.com";


        public ContextBuilder WithAccountId(string accountId)
        {
            _awsAccountId = accountId;
            return this;
        }
        
        public ContextBuilder WithRoleArn(string roleArn)
        {
            _awsRoleArn = roleArn;
            return this;
        }
        
        public ContextBuilder WithRoleEmail(string roleEmail)
        {
            _awsRoleEmail = roleEmail;
            return this;
        }
          
            
        public Context Build()
        {
            return new Context(_id, _name, _awsAccountId,_awsRoleArn,_awsRoleEmail);
        }
    }
}