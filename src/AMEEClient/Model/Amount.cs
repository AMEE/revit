using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class Amount
    {
        [JsonProperty("perUnit")]
        public string PerUnit { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; }
        [JsonProperty("default")]
        public bool Default { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}