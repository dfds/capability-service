using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;

namespace DFDS.TeamService.Tests.Builders
{
    public class UserBuilder
    {
        private string _id;
        private string _name;
        private string _email;

        public UserBuilder()
        {
            _id = "1";
            _name = "foo";
            _email = "bar";
        }

        public UserBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public User Build()
        {
            return new User(_id, _name, _email);
        }
    }
}