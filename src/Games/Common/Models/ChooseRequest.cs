using Newtonsoft.Json;

namespace JackboxGPT3.Games.Common.Models
{
    public struct ChooseRequest<TChoice>
    {
        public ChooseRequest(TChoice choice)
        {
            Choice = choice;
        }
        
        [JsonProperty("action")]
        public static string Action => "choose";

        [JsonProperty("choice")]
        public TChoice Choice { get; set; }
    }
}
