using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JackboxGPT3.Games.Quiplash3.Models
{
    public enum RoomState
    {
        Lobby,
        Logo,
        EnterSingleText,
        EnterTextList,
        MakeSingleChoice,
        // ReSharper disable once InconsistentNaming
        Gameplay_Logo
    }
    
    public struct Quiplash3Room
    {
        [JsonProperty("state")]
        public RoomState State { get; set; }
        
        // TODO: enum here?
        [JsonProperty("lobbyState")]
        public JRaw LobbyState { get; set; }
        
        [JsonProperty("characters")]
        public List<AvatarCharacter> Characters { get; set; }
    }

    public struct AvatarCharacter
    {
        [JsonProperty("available")]
        public bool Available { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
