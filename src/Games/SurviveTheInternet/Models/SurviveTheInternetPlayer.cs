using Newtonsoft.Json;

namespace JackboxGPT3.Games.SurviveTheInternet.Models
{
    public struct SurviveTheInternetPlayer
    {
        [JsonProperty("entry")]
        public bool Entry { get; set; }
        
        [JsonProperty("entryId")]
        public string EntryId { get; set; }

        [JsonProperty("inputType")]
        public string InputType { get; set; }

        [JsonProperty("maxLength")]
        public int MaxLength { get; set; }
    }
}