using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;

namespace DFDS.TeamService.Tests.Builders
{
    public class UserBuilder
    {
        public User Build()
        {
            return new User
            (
                id: "1",
                name: "foo",
                email: "foo@bar.com"
            );
        }
    }
}