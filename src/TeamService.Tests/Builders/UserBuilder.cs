using DFDS.TeamService.WebApi.Model;

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