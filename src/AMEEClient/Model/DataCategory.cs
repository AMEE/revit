using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class DataCategoryItem
    {
        [JsonProperty("uid")]
        public string Uid;
        [JsonProperty("dataCategory")]
        public DataCategoryItem DataCategory;
        [JsonProperty("deprecated")]
        public bool Deprecated;
        [JsonProperty("environment")]
        public Environment Environment;
        [JsonProperty("created")]
        public string Created;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("path")]
        public string Path;
        [JsonProperty("itemDefinition")]
        public ItemDefinition ItemDefinition;
        [JsonProperty("modified")]
        public string Modified;
    }
}