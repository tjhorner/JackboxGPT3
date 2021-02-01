using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JackboxGPT3.Games.Fibbage3.Models
{
    public struct Fibbage3Player
    {
        // ReSharper disable once UnusedMember.Global
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

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("isChoosing")]
        public bool IsChoosing { get; set; }

        [JsonProperty("playerAvatar")]
        public string PlayerAvatar { get; set; }

        [JsonProperty("playerIndex")]
        public int PlayerIndex { get; set; }

        [JsonProperty("playerName")]
        public string PlayerName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("entry")]
        public string Entry { get; set; }

        [JsonProperty("question")]
        public string Question { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("maxLength")]
        public int MaxLength { get; set; }

        [JsonProperty("choosingDone")]
        public bool? ChoosingDone { get; set; }

        [JsonProperty("state")]
        public RoomState State { get; set; }
    }
}
