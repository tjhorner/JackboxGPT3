using System;
using Newtonsoft.Json;

namespace JackboxGPT3.Games.Common.Models
{
    public struct GetRoomResponse
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("body")]
        public JackboxRoom Room { get; set; }
    }

    public struct JackboxRoom
    {
        [JsonProperty("appId")]
        public Guid AppId { get; set; }

        [JsonProperty("appTag")]
        public string AppTag { get; set; }

        [JsonProperty("audienceEnabled")]
        public bool AudienceEnabled { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("locked")]
        public bool Locked { get; set; }

        [JsonProperty("moderationEnabled")]
        public bool ModerationEnabled { get; set; }

        [JsonProperty("passwordRequired")]
        public bool PasswordRequired { get; set; }

        [JsonProperty("twitchLocked")]
        public bool TwitchLocked { get; set; }
    }
}
