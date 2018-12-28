using System.Net.Http;
using System.Text;

namespace DFDS.TeamService.Tests
{
    public class JsonContent : StringContent
    {
        public JsonContent(string content) 
            : base(content, Encoding.UTF8, "application/json")
        {
            
        }
    }
}