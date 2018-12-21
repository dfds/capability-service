using System;
using DFDS.TeamService.WebApi.Models;

namespace DFDS.TeamService.Tests.Builders
{
    public class TeamBuilder
    {
        private Guid _id;
        private string _name;

        public TeamBuilder()
        {
            _id = new Guid("11111111-1111-1111-1111-111111111111");
            _name = "foo";
        }

        public Team Build()
        {
            return new Team(_id, _name);
        }
    }
}