using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class ProfileItem
    {
        [JsonProperty("amount")]
        public Amount Amount;
        [JsonProperty("uid")]
        public string Uid;
        [JsonProperty("startDate")]
        public string StartDate;
        [JsonProperty("itemValues")]
        public ItemValue[] ItemValues;
        [JsonProperty("amounts")]
        public Amounts Amounts;
        [JsonProperty("created")]
        public string Created;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("endDate")]
        public string EndDate;
        [JsonProperty("dataItem")]
        public DataItem DataItem;
        [JsonProperty("modified")]
        public string Modified;
    }
    public class DataItemResponse
    {
        [JsonProperty("apiVersion")]
        public string ApiVersion;
        [JsonProperty("amount")]
        public Amount Amount;
        [JsonProperty("amounts")]
        public Amounts Amounts;
        [JsonProperty("path")]
        public string Path;
        [JsonProperty("amountPerMonth")]
        public string AmountPerMonth;
        [JsonProperty("userValueChoices")]
        public ChoicesCollection UserValueChoices;
        [JsonProperty("dataItem")]
        public DataItem DataItem;

    }
}