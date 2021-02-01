using Newtonsoft.Json;

namespace JackboxGPT3.Clients.Models
{
    public struct ClientWelcome
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}