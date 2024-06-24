using Newtonsoft.Json;

namespace AuthDemo.Dtos.Request
{
    public class TokenRequest
    {
        [JsonProperty(PropertyName = "grant_type")]
        public string Grant_Type { get; set; }
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "private_key")]
        public string Private_Key { get; set; }

    }
}
