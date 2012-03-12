using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class DataItem
    {
        [JsonProperty("uid")]
        public string Uid;
        [JsonProperty("startDate")]
        public string StartDate;
        [JsonProperty("dataCategory")]
        public DataCategoryItem DataCategory;
        [JsonProperty("itemValues")]
        public ItemValue[] ItemValues;
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
        [JsonProperty("endDate")]
        public string EndDate;
        [JsonProperty("label")]
        public string Label;
        [JsonProperty("modified")]
        public string Modified;
    }
}