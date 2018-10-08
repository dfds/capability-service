using System.Collections.Generic;

namespace DFDS.TeamService.WebApi.Model
{
    public class Team
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public List<User> Members { get; set; }
        
    }
}