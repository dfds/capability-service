namespace DFDS.TeamService.WebApi.Features.Teams.Infrastructure.Api
{
    public class ErrorMessage
    {
        public ErrorMessage(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}