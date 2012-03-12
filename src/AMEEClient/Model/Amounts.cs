using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class Amounts
    {
        [JsonProperty("amount")]
        public Amount[] Amount { get; set; }
        [JsonProperty("note")]
        public Note[] note { get; set; }
    }
}