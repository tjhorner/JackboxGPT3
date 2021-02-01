using Newtonsoft.Json;

namespace JackboxGPT3.Clients.Models.Fibbage3
{
    public struct ClientSendOperation<BodyType>
    {
        [JsonProperty("from")]
        public int From { get; set; }

        [JsonProperty("to")]
        public int To { get; set; }

        [JsonProperty("body")]
        public BodyType Body { get; set; }
    }
}
