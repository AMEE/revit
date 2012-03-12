using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class ItemValueDefinition
    {
        [JsonProperty("perUnit")]
        public string PerUnit;
        [JsonProperty("uid")]
        public string Uid;
        [JsonProperty("unit")]
        public string Unit;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("fromData")]
        public bool FromData;
        [JsonProperty("path")]
        public string Path;
        [JsonProperty("fromProfile")]
        public bool FromProfile;
        [JsonProperty("drillDown")]
        public bool DrillDown;
        [JsonProperty("valueDefinition")]
        public ValueDefinition ValueDefinition;
    }
}