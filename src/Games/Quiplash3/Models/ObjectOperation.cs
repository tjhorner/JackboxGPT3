using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JackboxGPT3.Games.Quiplash3.Models
{
    public struct ObjectOperation
    {
        [JsonProperty("from")]
        public int From { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("val")]
        public JRaw Value { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }
}