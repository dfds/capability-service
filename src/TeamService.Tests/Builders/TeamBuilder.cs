using System.Collections.Generic;
using DFDS.TeamService.WebApi.Model;

namespace DFDS.TeamService.Tests.Builders
{
    public class TeamBuilder
    {
        private string _id;
        private string _name;
        private string _department;
        private List<User> _members;

        public TeamBuilder()
        {
            _id = "1";
            _name = "foo";
            _department = "bar";
            _members = new List<User>();
        }

        public TeamBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public TeamBuilder WithMembers(params User[] members)
        {
            _members = new List<User>(members);
            return this;
        }

        public Team Build()
        {
            return new Team
            {
                Id = _id,
                Name = _name,
                Department = _department,
                Members = _members
            };
        }
    }
}