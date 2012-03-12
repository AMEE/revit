using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class Environment
    {
        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("itemsPerFeed")]
        public int ItemsPerFeed { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }
    }
}