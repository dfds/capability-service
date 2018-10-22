using System;
using System.Text.RegularExpressions;

namespace DFDS.TeamService.WebApi.Features.AwsRoles
{
    public class TeamNameToRoleNameConverter
    {
        // https://docs.aws.amazon.com/IAM/latest/APIReference/API_CreateRole.html
        public string Convert(string teamName)
        {

            if (teamName.Contains("+"))
            {
                throw new ArgumentException($"{nameof(teamName)} may not contain plus signs");

            }
            if (teamName.Contains(" "))
            {
                teamName = teamName.Replace(" ", "+");
            }
            
            var regex = new Regex(@"[\w=,.@-]+");
            var matchResult = regex.Match(teamName);

            if (matchResult.Success == false)
            {
                throw new ArgumentException($"{nameof(teamName)} must be match the regular expression: [ \\w=,.@-]+");
            }

            var maxTeamNameLength = 64;
            if (maxTeamNameLength < teamName.Length)
            {
                throw new ArgumentException(
                    $"the safe version of {nameof(teamName)} is over the maximum length of {maxTeamNameLength}");
            }

            
            return teamName;
        }
    }
}