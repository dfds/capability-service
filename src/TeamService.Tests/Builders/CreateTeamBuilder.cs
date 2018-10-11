using System.Net.Http;
using DFDS.TeamService.WebApi.Features.Teams;

namespace DFDS.TeamService.Tests.Builders
{
    public class CreateTeamBuilder
    {
        private string _name;
        private string _department;

        public CreateTeamBuilder()
        {
            _name = "foo";
            _department = "bar";
        }

        public CreateTeamBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public CreateTeam Build()
        {
            return new CreateTeam
            {
                Name = _name,
                Department = _department
            };
        }

        public HttpContent BuildAsJsonContent()
        {
            return new JsonContent(Build());
        }
    }
}