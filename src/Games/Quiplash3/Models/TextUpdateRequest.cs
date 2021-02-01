using Newtonsoft.Json;

namespace JackboxGPT3.Games.Quiplash3.Models
{
    public struct TextUpdateRequest
    {
        public TextUpdateRequest(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [JsonIgnore]
        public const string OpCode = "text/update";

        [JsonProperty("key")]
        public static string Key { get; set; }
        
        [JsonProperty("val")]
        public string Value { get; set; }
    }
}