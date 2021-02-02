using Newtonsoft.Json;

namespace JackboxGPT3.Games.WordSpud.Models
{
    public struct WordSpudPlayer
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("state")]
        public RoomState State { get; set; }
    }
}