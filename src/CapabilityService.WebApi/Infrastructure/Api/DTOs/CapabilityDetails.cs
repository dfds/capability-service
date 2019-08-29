using System;
using System.Collections.Generic;
using System.Linq;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs
{
    public class CapabilityDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string RootId { get; set; }
        public string TopicCommonPrefix { get; set; }
        public string Description { get; set; }
        public Member[] Members { get; set; }
        public Context[] Contexts { get; set; }

        public Topic[] Topics { get; set; }

        public static CapabilityDetails Create(Domain.Models.Capability capability, IEnumerable<Domain.Models.Topic> topics)
        {
            return new CapabilityDetails
            {
                Id = capability.Id,
                Name = capability.Name,
                RootId = capability.RootId,
                Description = capability.Description,
                TopicCommonPrefix = capability.TopicCommonPrefix,
                Members = capability
                    .Members
                    .Select(member => new Member {Email = member.Email})
                    .ToArray(),
                Contexts = capability
                    .Contexts
                    .Select(context => new Context
                    {
                        Id = context.Id,
                        Name = context.Name,
                        AWSRoleArn = context.AWSRoleArn,
                        AWSAccountId = context.AWSAccountId,
                        AWSRoleEmail = context.AWSRoleEmail
                    })
                    .ToArray(),
                Topics = topics
                    .Select(Topic.CreateFrom)
                    .ToArray(),
            };
        }
    }
}