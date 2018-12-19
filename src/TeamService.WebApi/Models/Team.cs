using System;

namespace DFDS.TeamService.WebApi.Models
{
    public class Team
    {
        public Team(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
    }
}