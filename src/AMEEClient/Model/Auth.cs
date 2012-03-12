using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class Auth
    {
        [JsonProperty("uid")]
        public string Uid;
        [JsonProperty("username")]
        public string Username;  
    }
}