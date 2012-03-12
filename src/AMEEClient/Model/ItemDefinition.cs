using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class ItemDefinition
    {
        [JsonProperty("uid")]
        public string Uid;
        [JsonProperty("environment")]
        public Environment Environment;
        [JsonProperty("created")]
        public string Created;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("drillDown")]
        public string DrillDown;
        [JsonProperty("modified")]
        public string Modified;
    }
}