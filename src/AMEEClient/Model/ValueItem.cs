using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class ValueItem
    {
        public ValueItem()
        {
        }

        public ValueItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
 
    }
}