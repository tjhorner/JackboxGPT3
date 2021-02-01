using Newtonsoft.Json;

namespace JackboxGPT3.Games.Quiplash3.Models
{
    public struct ChooseAvatarRequest
    {
        public ChooseAvatarRequest(string name)
        {
            Name = name;
        }
        
        [JsonProperty("action")]
        public static string Action => "avatar";
        
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}