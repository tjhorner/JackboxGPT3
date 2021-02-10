using Newtonsoft.Json;

namespace JackboxGPT3.Games.SurviveTheInternet.Models
{
    public struct WriteEntryRequest
    {
        public WriteEntryRequest(string entry)
        {
            Entry = entry;
        }
        
        [JsonProperty("action")]
        public static string Action => "write";
        
        [JsonProperty("entry")]
        public string Entry { get; set; }
    }
}