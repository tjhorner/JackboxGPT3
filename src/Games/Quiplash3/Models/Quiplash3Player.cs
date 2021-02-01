using System.Collections.Generic;
using Newtonsoft.Json;

namespace JackboxGPT3.Games.Quiplash3.Models
{
    public struct Quiplash3Player
    {
        [JsonProperty("playerInfo")]
        public PlayerInfo Info { get; set; }
        
        [JsonProperty("state")]
        public RoomState State { get; set; }
        
        [JsonProperty("prompt", NullValueHandling = NullValueHandling.Ignore)]
        public HtmlValue Prompt { get; set; }
        
        [JsonProperty("choices")]
        public List<HtmlValue> Choices { get; set; }
        
        [JsonProperty("choiceType")]
        public ChoiceType ChoiceType { get; set; }
        
        [JsonProperty("textKey")]
        public string TextKey { get; set; }
        
        [JsonProperty("entries")]
        public bool Entries { get; set; }
    }

    public enum ChoiceType
    {
        // For normal answers
        ChoseQuip,
        // For thriplash (is this just a typo? lol)
        ChooseQuip
    }

    public struct PlayerInfo
    {
        [JsonProperty("index")]
        public int Index { get; set; }
        
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        
        // What is the difference between Index and PlayerIndex? idk lmao
        [JsonProperty("playerIndex")]
        public int PlayerIndex { get; set; }
        
        [JsonProperty("sessionId")]
        public int SessionId { get; set; }
        
        [JsonProperty("username")]
        public string Username { get; set; }
    }

    public struct HtmlValue
    {
        [JsonProperty("html")]
        public string Html { get; set; }
        
        [JsonProperty("key")]
        public int Key { get; set; }
    }
}
