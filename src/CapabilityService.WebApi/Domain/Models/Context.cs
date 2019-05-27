using System;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class Context : Entity<Guid>
    {
        private Context (){}
        public string Name { get; private set; }
        
        public string AWSAccountId { get; private set; }
        public string AWSRoleArn { get; private set; }
        public string AWSRoleEmail { get; private set; }
        
        public Context(Guid id, string name, string awsAccountId = "", string awsRoleArn ="", string awsRoleEmail ="")
        {
            Id = id;
            Name = name;
            AWSAccountId = awsAccountId;
            AWSRoleArn = awsRoleArn;
            AWSRoleEmail = awsRoleEmail;
        }
        
        public void UpdateContext(string awsAccountId, string awsRoleArn, string awsRoleEmail)
        {
            AWSAccountId = awsAccountId;
            AWSRoleArn = awsRoleArn;
            AWSRoleEmail = awsRoleEmail;
        }
    }
}