using System;
using System.Collections.Generic;
using System.Linq;

namespace DFDS.TeamService.WebApi.Models
{
    public class Team
    {
        private readonly List<Member> _members = new List<Member>();

        private Team()
        {
            
        }

        public Team(Guid id, string name, IEnumerable<Member> members)
        {
            Id = id;
            Name = name;
            _members.AddRange(members);
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<Member> Members => _members;

        public static Team Create(string name)
        {
            return new Team(
                id: Guid.NewGuid(),
                name: name,
                members: Enumerable.Empty<Member>()
            );
        }
    }

    public class Member
    {
        private Member()
        {
            
        }

        public Member(string email)
        {
            Email = email;
        }

        public string Email { get; private set; }
    }
}