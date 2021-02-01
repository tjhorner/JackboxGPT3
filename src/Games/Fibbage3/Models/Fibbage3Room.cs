using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JackboxGPT3.Games.Fibbage3.Models
{
    public enum LobbyState
    {
        WaitingForMore,
        CanStart,
        Countdown,
        PostGame
    }

    public enum RoomState
    {
        Lobby,
        Logo,
        CategorySelection,
        EnterText,
        ChooseLie,
        EndShortie,
        ChooseLike,
        EndGame
    }

    public struct Fibbage3Room
    {
        public List<string> CategoryChoices
        {
            get
            {
                if (State != RoomState.CategorySelection)
                    return new List<string>();

                var parsed = JsonConvert.DeserializeObject<List<string>>(Choices.ToString());
                return parsed;
            }
        }

        public List<LieChoice> LieChoices
        {
            get
            {
                if (State != RoomState.ChooseLie)
                    return new List<LieChoice>();

                var parsed = JsonConvert.DeserializeObject<List<LieChoice>>(Choices.ToString());
                return parsed;
            }
        }

        [JsonProperty("choices")]
        public JRaw Choices { get; set; }

        [JsonProperty("isLocal")]
        public bool IsLocal { get; set; }

        [JsonProperty("lobbyState")]
        public LobbyState? LobbyState { get; set; }

        [JsonProperty("platformId")]
        public string PlatformId { get; set; }

        [JsonProperty("round")]
        public double Round { get; set; }

        [JsonProperty("state")]
        public RoomState State { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("question")]
        public string Question { get; set; }
    }
}
