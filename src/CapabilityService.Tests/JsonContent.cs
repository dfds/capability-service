using System.Net.Http;
using System.Text;

namespace DFDS.CapabilityService.Tests
{
    public class JsonContent : StringContent
    {
        public JsonContent(string content) 
            : base(content, Encoding.UTF8, "application/json")
        {
            
        }
    }
}