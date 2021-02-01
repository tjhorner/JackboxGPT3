using Newtonsoft.Json;

namespace JackboxGPT3.Games.Quiplash3.Models
{
    public enum RoomState
    {
        Lobby,
        EnterSingleText,
        EnterTextList,
        MakeSingleChoice
    }
    
    public struct Quiplash3Room
    {
        [JsonProperty("state")]
        public RoomState State { get; set; }
        
        // TODO: enum here
        [JsonProperty("lobbyState")]
        public string LobbyState { get; set; }
    }
}
