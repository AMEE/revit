using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class CreateProfileResponse
    {
        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; }

        [JsonProperty("profile")]
        public Profile Profile { get; set; }
    }
}