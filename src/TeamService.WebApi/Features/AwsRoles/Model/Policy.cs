namespace DFDS.TeamService.WebApi.Features.AwsRoles.Model
{
    public class Policy
    {
        public Policy(string name, string document)
        {
            Name = name;
            Document = document;
        }
        
        public string Name { get; }
        public string Document { get; }
    }
}