namespace DFDS.TeamService.WebApi.Features.Teams.Domain.Models
{
    public class User : Entity<string>
    {
        public User(string id, string name, string email) : base(id)
        {
            Name = name;
            Email = email;
        }

        public string Name { get; private set; }
        public string Email { get; private set; }
    }
}