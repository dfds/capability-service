using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace DFDS.TeamService.Tests
{
    public class JsonContent : StringContent
    {
        public JsonContent(object instance)
            : base(ConvertToJson(instance), Encoding.UTF8, "application/json")
        {

        }

        public static string ConvertToJson(object instance)
        {
            if (instance == null)
            {
                return "{ }";
            }

            return JsonConvert.SerializeObject(instance);
        }

        public static JsonContent Empty => new JsonContent(null);
    }
}