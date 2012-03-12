using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class User
    {
        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}