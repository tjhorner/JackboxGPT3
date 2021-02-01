using Newtonsoft.Json;

namespace JackboxGPT3.Games.Quiplash3.Models
{
    public struct Quiplash3Player
    {
        [JsonProperty("playerInfo")]
        public PlayerInfo Info { get; set; }
        
        [JsonProperty("state")]
        public RoomState State { get; set; }
    }

    public struct PlayerInfo
    {
        [JsonProperty("index")]
        public int Index { get; set; }
        
        // What is the difference between Index and PlayerIndex? idk lmao
        [JsonProperty("playerIndex")]
        public int PlayerIndex { get; set; }
        
        [JsonProperty("sessionId")]
        public int SessionId { get; set; }
        
        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
