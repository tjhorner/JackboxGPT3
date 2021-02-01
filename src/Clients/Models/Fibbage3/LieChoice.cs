using Newtonsoft.Json;

namespace JackboxGPT3.Clients.Models.Fibbage3
{
    public struct LieChoice
    {
        [JsonProperty("disabled")]
        public bool Disabled { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
