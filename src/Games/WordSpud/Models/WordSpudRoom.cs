using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JackboxGPT3.Games.WordSpud.Models
{
    public struct WordSpudRoom
    {
        [JsonProperty("currentWord")]
        public string CurrentWord { get; set; }
        
        [JsonProperty("spud")]
        public string Spud { get; set; }
        
        [JsonProperty("state")]
        public RoomState State { get; set; }
    }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoomState
    {
        Gameplay,
        [EnumMember(Value = "Gameplay_Enter")]
        GameplayEnter,
        
        Lobby,
        [EnumMember(Value = "Lobby_CanStart")]
        LobbyCanStart,
        Vote,
        Voted,
        
        RoundOver,
        GameOver,
        
    }
}