using System;

namespace DFDS.TeamService.WebApi.Models
{
    public class Team
    {
        private Team()
        {
            
        }

        public Team(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public static Team Create(string name)
        {
            return new Team(
                id: Guid.NewGuid(),
                name: name
            );
        }
    }
}