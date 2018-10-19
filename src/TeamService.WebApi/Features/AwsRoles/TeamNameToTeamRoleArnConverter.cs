using System;
using System.Text.RegularExpressions;

namespace DFDS.TeamService.WebApi.Features.AwsRoles
{
    public class TeamNameToTeamRoleArnConverter
    {
        private readonly string _awsAccountIdNumber;

        public TeamNameToTeamRoleArnConverter(string awsAccountIdNumber)
        {
            var regex = new Regex("^[0-9]{12}$");
            var matchResult = regex.Match(awsAccountIdNumber);

            if (matchResult.Success == false)
            {
                throw new ArgumentException($"{nameof(awsAccountIdNumber)} must be a 12 digits number");
            }
            
            _awsAccountIdNumber = awsAccountIdNumber;
        }
        
        
        // https://docs.aws.amazon.com/IAM/latest/APIReference/API_CreateRole.html
        public string Convert(string teamName)
        {

            if (teamName.Contains(".."))
            {
                throw new ArgumentException($"{nameof(teamName)} may not contain double dots");

            }
            if (teamName.Contains(" "))
            {
                teamName = teamName.Replace(" ", "..");
            }
            
            var regex = new Regex(@"[\w+=,.@-]+");
            var matchResult = regex.Match(teamName);

            if (matchResult.Success == false)
            {
                throw new ArgumentException($"{nameof(teamName)} must be match the regex: [ \\w+=,.@-]+");
            }

            var maxTeamNameLength = 64;
            if (maxTeamNameLength < teamName.Length)
            {
                throw new ArgumentException(
                    $"the safe version of {nameof(teamName)} is over the maximum length of {maxTeamNameLength}");
            }

            
            return $"arn:aws:iam::{_awsAccountIdNumber}:role/{teamName}";
        }
    }
}