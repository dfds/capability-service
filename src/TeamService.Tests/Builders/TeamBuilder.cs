using System;
using System.Collections.Generic;
using System.Linq;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;

namespace DFDS.TeamService.Tests.Builders
{
    public class TeamBuilder
    {
        private Guid _id;
        private string _name;
        private string _department;
        private List<Membership> _members;

        public TeamBuilder()
        {
            _id = Guid.Empty;
            _name = "foo";
            _department = "bar";
            _members = new List<Membership>();
        }

        public TeamBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public TeamBuilder WithMembers(params User[] members)
        {
            _members = new List<Membership>(members.Select(x => Membership.Start(x, MembershipType.Unknown)));
            return this;
        }

        public Team Build()
        {
            return new Team
            (
                id: _id,
                name: _name,
                department: _department,
                memberships: _members
            );
        }
    }

    public class MembershipBuilder
    {
        private Guid _id;
        private Team _team;
        private User _user;
        private MembershipType _type;
        private DateTime _startedDate;

        public MembershipBuilder()
        {
            _id = Guid.Empty;
            _team = new TeamBuilder().Build();
            _user = new UserBuilder().Build();
            _type = MembershipType.Unknown;
            _startedDate = new DateTime(2000, 1, 1);
        }

        public Membership Build()
        {
            return new Membership(_id, _user, _type, _startedDate);
        }
    }

}