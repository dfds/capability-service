namespace DFDS.TeamService.WebApi.Failures
{
    public class Conflict: IFailure
    {
        public Conflict(string message)
        {
            Message = message;
        }
        public string Message { get; }
    }
}