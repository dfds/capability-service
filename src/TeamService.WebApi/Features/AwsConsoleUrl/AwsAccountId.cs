using System;

namespace DFDS.TeamService.WebApi.Features.AwsConsoleLogin
{
    public class AwsAccountId
    {
        private readonly string _awsAccountId;
        private static readonly ArgumentException twelveDigitsException = new ArgumentException("awsAccountId must be 12 a digits number");

        public AwsAccountId(string awsAccountId)
        {
            if (awsAccountId.Length != 12)
            {
                throw twelveDigitsException;
            }
            
            for (var i = 0; i < awsAccountId.Length; ++i) {
                var currentChar = awsAccountId[i];
                if (currentChar < '0' || currentChar > '9')
                    throw twelveDigitsException;
            }
            
            _awsAccountId = awsAccountId;
        }
        
        public override string ToString()
        {
            return _awsAccountId;
        }
    }
}