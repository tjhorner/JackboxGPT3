using Newtonsoft.Json;

namespace JackboxGPT3.Games.Fibbage3.Models
{
    public struct SendEntryRequest
    {
        [JsonProperty("entry")]
        public string Entry { get; set; }
    }
}
