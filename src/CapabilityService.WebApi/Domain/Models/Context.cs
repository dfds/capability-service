using System;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public class Context : Entity<Guid>
    {
        private Context (){}
        public string Name { get; private set; }
        
        public Context(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}