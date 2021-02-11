using System.Collections.Generic;
using Newtonsoft.Json;

namespace JackboxGPT3.Games.BlatherRound.Models
{
    public struct TextDescription
    {
        [JsonProperty("category")]
        public string Category { get; set; }
        
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public struct TextDescriptions
    {
        [JsonProperty("latestDescriptions")]
        public IList<TextDescription> LatestDescriptions { get; set; }
    }
}