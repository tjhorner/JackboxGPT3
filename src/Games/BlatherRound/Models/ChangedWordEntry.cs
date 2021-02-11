using System.Collections.Generic;
using Newtonsoft.Json;

namespace JackboxGPT3.Games.BlatherRound.Models
{
    public struct ChangedWordEntry
    {
        [JsonProperty("changedWord")]
        public string ChangedWord { get; set; }
        
        [JsonProperty("position")]
        public WordPosition Position { get; set; }

        public struct WordPosition
        {
            [JsonProperty("blank")]
            public static int Blank => 0;
            
            [JsonProperty("sentence")]
            public int Sentence { get; set; }
        }
        
        [JsonProperty("sentence")]
        public IList<string> Sentence { get; set; }
    }
}