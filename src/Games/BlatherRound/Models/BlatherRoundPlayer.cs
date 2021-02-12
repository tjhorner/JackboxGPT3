using System.Collections.Generic;
using Newtonsoft.Json;

namespace JackboxGPT3.Games.BlatherRound.Models
{
    public struct BlatherRoundPlayer
    {
        [JsonProperty("state")]
        public PlayerState State { get; set; }
        
        [JsonProperty("sentence")]
        public Sentence Sentence { get; set; }
        
        [JsonProperty("choices")]
        public List<HtmlEncoded> Choices { get; set; }
        
        [JsonProperty("prompt")]
        public HtmlEncoded Prompt { get; set; }
        
        [JsonProperty("entryId")]
        public string EntryId { get; set; }
        
        [JsonProperty("textKey")]
        public string TextKey { get; set; }
    }

    public struct HtmlEncoded
    {
        [JsonProperty("html")]
        public string Html { get; set; }
        
        [JsonProperty("className")]
        public string ClassName { get; set; }
    }

    public enum PlayerState
    {
        Lobby,
        MakeSingleChoice,
        Logo,
        Gameplay_Logo,
        MakeSentence,
        EnterSingleText
    }
}