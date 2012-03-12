using System.Collections.Generic;
using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class ChoicesCollection
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("choices")]
        public List<ValueItem> Choices { get; set; }
    }
}