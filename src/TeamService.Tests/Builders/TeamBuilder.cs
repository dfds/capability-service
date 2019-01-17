using System;
using System.Linq;
using DFDS.TeamService.WebApi.Models;

namespace DFDS.TeamService.Tests.Builders
{
    public class TeamBuilder
    {
        private Guid _id;
        private string _name;
        private Membership[] _memberships;

        public TeamBuilder()
        {
            _id = new Guid("11111111-1111-1111-1111-111111111111");
            _name = "foo";
            _memberships = new Membership[0];
        }

        public TeamBuilder WithMembers(params string[] members)
        {
            _memberships = members
                       .Select(email => Membership.StartFor(new Member(email)))
                       .ToArray();

            return this;
        }

        public Team Build()
        {
            return new Team(_id, _name, _memberships);
        }
    }
}