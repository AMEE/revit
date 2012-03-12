using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class ValueDefinition
    {
        [JsonProperty("uid")]
        public string Uid;
        [JsonProperty("environment")]
        public Environment Environment;
        [JsonProperty("created")]
        public string Created;
        [JsonProperty("description")]
        public string Description;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("valueType")]
        public string ValueType;
        [JsonProperty("modified")]
        public string Modified;
    }
}