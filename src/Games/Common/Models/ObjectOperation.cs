using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JackboxGPT3.Games.Common.Models
{
    public struct ObjectOperation : IOperation
    {
        [JsonProperty("from")]
        public int From { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("val")]
        public JRaw RawValue { get; set; }

        public string Value => RawValue.ToString();

        [JsonProperty("version")]
        public int Version { get; set; }
    }
}