using System;

namespace DFDS.CapabilityService.WebApi.Features.Capabilities.Infrastructure.Api.DTOs
{
    public class Context
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AWSAccountId { get; set; }
        public string AWSRoleArn { get; set; }
        public string AWSRoleEmail { get; set; }
    }
}