using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class Profile
    {
        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("environment")]
        public Environment Environment { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("permission")]
        public Permission Permission;

        [JsonProperty("modified")]
        public string Modified { get; set; }
    }
}