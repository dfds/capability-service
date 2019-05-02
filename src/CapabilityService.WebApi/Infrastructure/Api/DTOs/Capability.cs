using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs
{
    public class Capability
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Member[] Members { get; set; }
        
        public Context[] Contexts { get; set; }
        
        public static Capability Create(Domain.Models.Capability capability)
        {
            return new Capability
            {
                Id = capability.Id,
                Name = capability.Name,
                Description = capability.Description,
                Members = capability
                    .Members
                    .Select(member => new Member {Email = member.Email})
                    .ToArray(),
                Contexts = capability
                    .Contexts
                    .Select(context => new Context{Id = context.Id, Name = context.Name})
                    .ToArray()
            };
        }
    }
}