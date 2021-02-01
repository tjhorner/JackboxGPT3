using Newtonsoft.Json;

namespace JackboxGPT3.Clients.Models.Fibbage3
{
    public struct SendEntryRequest
    {
        [JsonProperty("entry")]
        public string Entry { get; set; }
    }
}
