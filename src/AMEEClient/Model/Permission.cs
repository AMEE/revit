using Newtonsoft.Json;

namespace AMEEClient.Model
{
    public class Permission
    {
        [JsonProperty("uid")]
        public string Uid;
        [JsonProperty("created")]
        public string Created;
        [JsonProperty("group")]
        public Group Group;
        [JsonProperty("environmentUid")]
        public string EnvironmentUid;
        [JsonProperty("auth")]
        public Auth Auth;
        [JsonProperty("modified")]
        public string Modified;
    }
}