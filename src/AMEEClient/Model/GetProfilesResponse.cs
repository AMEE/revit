using AMEEClient.Util;
using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class GetProfilesResponse
    {
        //{
        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; }

        [JsonProperty("pager")]
        public Pager Pager { get; set; }

        [JsonProperty("profiles")]
        public Profile[] Profiles { get; set; }
    }
}