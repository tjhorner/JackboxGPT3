using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JackboxGPT3.Games.SurviveTheInternet.Models
{
    public struct SurviveTheInternetPlayer
    {
        public List<ImageChoice> ImageChoices
        {
            get
            {
                if (State != RoomState.MakeSingleChoice)
                    return new List<ImageChoice>();

                var parsed = JsonConvert.DeserializeObject<List<ImageChoice>>(Choices.ToString());
                return parsed;
            }
        }
        
        public List<EntryChoice> EntryChoices
        {
            get
            {
                if (State != RoomState.Voting)
                    return new List<EntryChoice>();

                var parsed = JsonConvert.DeserializeObject<List<EntryChoice>>(Choices.ToString());
                return parsed;
            }
        }
        
        [JsonProperty("state")]
        public RoomState State { get; set; }
        
        /// <summary>
        /// Has the entry been submitted?
        /// </summary>
        [JsonProperty("entry")]
        public bool Entry { get; set; }
        
        [JsonProperty("entryId")]
        public string EntryId { get; set; }

        /// <summary>
        /// Always "textarea" from what I've seen.
        /// </summary>
        [JsonProperty("inputType")]
        public string InputType { get; set; }

        [JsonProperty("maxLength")]
        public int MaxLength { get; set; }
        
        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }
        
        [JsonProperty("text")]
        public TextPrompt Text { get; set; }
        
        [JsonProperty("choices")]
        public JRaw Choices { get; set; }
        
        [JsonProperty("chosen")]
        public int? Chosen { get; set; }
    }

    public struct ImageChoice
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public struct EntryChoice
    {
        [JsonProperty("body")]
        public string Body { get; set; }
        
        [JsonProperty("footer")]
        public string Footer { get; set; }
        
        [JsonProperty("header")]
        public string Header { get; set; }
    }

    public struct TextPrompt
    {
        [JsonProperty("aboveBlackBox")]
        public string AboveBlackBox { get; set; }
        
        [JsonProperty("belowBlackBox")]
        public string BelowBlackBox { get; set; }
        
        [JsonProperty("blackBox")]
        public string BlackBox { get; set; }
        
        [JsonProperty("prefix")]
        public string Prefix { get; set; }
        
        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        public static implicit operator TextPrompt(string str) => new() {BlackBox = str};
    }
}