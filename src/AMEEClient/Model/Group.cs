using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class Group
    {
        [JsonProperty("uid")]
        public string Uid;
        [JsonProperty("name")]
        public string Name;
    }
}