using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JackboxGPT3.Games.BlatherRound
{
    public class SentencePart
    {
        [JsonProperty("choices")]
        public List<string> Choices { get; set; }

        [JsonProperty("maxChoices")]
        public int MaxChoices { get; set; }
        
        [JsonProperty("optional")]
        public bool Optional { get; set; }
        
        // this can either be a string or an array of one string (why??)
        [JsonProperty("placeholder")]
        public JRaw Placeholder { get; set; }

        [JsonIgnore] public bool HasChoice = false;
        [JsonIgnore] public int CurrentChoiceIndex = 0;
        [JsonIgnore] public string CurrentChoice => Choices[CurrentChoiceIndex];
        [JsonIgnore] public bool ShouldChoose => Choices.Count > 1 && !Optional;
    }
}