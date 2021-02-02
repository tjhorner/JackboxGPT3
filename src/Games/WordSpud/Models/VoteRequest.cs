using Newtonsoft.Json;

namespace JackboxGPT3.Games.WordSpud.Models
{
    public struct VoteRequest
    {
        public VoteRequest(int vote)
        {
            Vote = vote;
        }
        
        [JsonProperty("vote")]
        public int Vote { get; set; }
    }
}