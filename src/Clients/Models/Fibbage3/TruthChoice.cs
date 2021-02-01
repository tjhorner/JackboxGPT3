using Newtonsoft.Json;

namespace JackboxGPT3.Clients.Models.Fibbage3
{
    public struct TruthChoice
    {
        [JsonProperty("censorable")]
        public bool Censorable { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }

        [JsonProperty("disabled")]
        public bool Disabled { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
