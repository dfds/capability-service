using System;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class Context : Entity<Guid>
    {
        public string Name { get; }
        
        public Context(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}