using System;
using Newtonsoft.Json;

namespace JackboxGPT3.Clients.Models.Fibbage3
{
    public struct ChooseRequest<ChoiceType>
    {
        [JsonProperty("action")]
        public string Action => "choose";

        [JsonProperty("choice")]
        public ChoiceType Choice { get; set; }
    }
}
