using Newtonsoft.Json;

namespace AMEEClient.Util
{
    public class Choice
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }

    }
    public class Pager
    {

        [JsonProperty("to")]
        public int To { get; set; }
        [JsonProperty("lastPage")]
        public int LastPage { get; set; }
        [JsonProperty("nextPage")]
        public int NextPage { get; set; }
        [JsonProperty("items")]
        public int Items { get; set; }
        [JsonProperty("startto")]
        public int Start { get; set; }
        [JsonProperty("itemsFound")]
        public int ItemsFound { get; set; }
        [JsonProperty("requestedPage")]
        public int RequestedPage { get; set; }
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }
        [JsonProperty("from")]
        public int From { get; set; }
        [JsonProperty("itemsPerPage")]
        public int ItemsPerPage { get; set; }
        [JsonProperty("previousPage")]
        public int PreviousPage { get; set; }
        
        
    }
}