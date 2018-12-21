using System;

namespace DFDS.TeamService.WebApi.Models.DTOs
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Member[] Members { get; set; }
    }

    public class Member
    {
        public string Email { get; set; }
    }
}