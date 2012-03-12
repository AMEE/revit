using System.Collections.Generic;
using Newtonsoft.Json;

namespace AMEEClient.Model
{
    /// <summary>
    /// #TODO: not fully implemented
    /// </summary>
    public class DrillDownResponse
    {
        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; }

        [JsonProperty("dataCategory")]
        public DataCategoryItem DataCategory { get; set; }
        /// <summary>
        /// shows what choices are available for the next level of drill.
        /// </summary>
        [JsonProperty("choices")]
        public ChoicesCollection Choices { get; set; }

        /// <summary>
        /// This shows choices that have already been made and specified in the drilldown URL
        /// </summary>
        [JsonProperty("selections")]
        public List<ValueItem> Selections { get; set; }



        [JsonProperty("itemDefinition")]
        public ItemDefinition ItemDefinition { get; set; }

    }
}