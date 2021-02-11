using System.Collections.Generic;
using Newtonsoft.Json;

namespace JackboxGPT3.Games.BlatherRound.Models
{
    public struct SubmitSentenceEntry
    {
        public SubmitSentenceEntry(IList<string> sentence)
        {
            Position = new SentencePosition();
            Sentence = sentence;
        }
        
        [JsonProperty("position")]
        public SentencePosition Position { get; set; }

        public struct SentencePosition
        {
            [JsonProperty("sentence")]
            public static int Sentence => -1;
        }
            
        [JsonProperty("sentence")]
        public IList<string> Sentence { get; set; }
    }
}