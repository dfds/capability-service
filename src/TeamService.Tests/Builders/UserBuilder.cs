using DFDS.TeamService.WebApi.Features.Teams;

namespace DFDS.TeamService.Tests.Builders
{
    public class UserBuilder
    {
        public User Build()
        {
            return new User
            {
                Id = "1",
                Name = "foo",
                Email = "foo@bar.com"
            };
        }
    }
}