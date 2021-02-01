using Newtonsoft.Json;

namespace JackboxGPT3.Games.Fibbage3.Models
{
    public struct LieChoice
    {
        [JsonProperty("disabled")]
        public bool Disabled { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
