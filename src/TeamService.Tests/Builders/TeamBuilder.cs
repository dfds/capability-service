using System;
using System.Linq;
using DFDS.TeamService.WebApi.Models;

namespace DFDS.TeamService.Tests.Builders
{
    public class TeamBuilder
    {
        private Guid _id;
        private string _name;
        private Member[] _members;

        public TeamBuilder()
        {
            _id = new Guid("11111111-1111-1111-1111-111111111111");
            _name = "foo";
            _members = new Member[0];
        }

        public TeamBuilder WithMembers(Member[] members)
        {
            _members = members;
            return this;
        }

        public TeamBuilder WithMembers(params string[] members)
        {
            _members = members
                       .Select(email => new Member(email))
                       .ToArray();

            return this;
        }

        public Team Build()
        {
            return new Team(_id, _name, _members);
        }
    }
}