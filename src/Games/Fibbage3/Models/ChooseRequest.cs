using Newtonsoft.Json;

namespace JackboxGPT3.Games.Fibbage3.Models
{
    public struct ChooseRequest<TChoice>
    {
        [JsonProperty("action")]
        public string Action => "choose";

        [JsonProperty("choice")]
        public TChoice Choice { get; set; }
    }
}
