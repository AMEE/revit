using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class ItemValue
    {
        [JsonProperty("path")]
        public string Path;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("value")]
        public string Value;
        [JsonProperty("unit")]
        public string Unit;
        [JsonProperty("perUnit")]
        public string PerUnit;
        [JsonProperty("startDate")]
        public string StartDate;
        [JsonProperty("itemValueDefinition")]
        public ItemValueDefinition ItemValueDefinition;
    }
}