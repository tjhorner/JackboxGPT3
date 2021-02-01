using Newtonsoft.Json;

namespace JackboxGPT3.Games.Common.Models
{
    public struct BootstrapPayload
    {
        [JsonProperty("role")]
        [JsonRequired]
        public string Role { get; set; }

        [JsonProperty("name")]
        [JsonRequired]
        public string Name { get; set; }

        [JsonProperty("user-id")]
        [JsonRequired]
        public string UserId { get; set; }

        [JsonProperty("format")]
        [JsonRequired]
        public string Format { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
