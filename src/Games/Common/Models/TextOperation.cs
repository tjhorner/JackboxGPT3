using Newtonsoft.Json;

namespace JackboxGPT3.Games.Common.Models
{
    public struct TextOperation : IOperation
    {
        [JsonProperty("from")]
        public int From { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("val")]
        public string Value { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }
}
