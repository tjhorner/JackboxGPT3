using Newtonsoft.Json;

namespace JackboxGPT3.Clients.Models
{
    public struct ClientMessageOperation<BodyType>
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("opcode")]
        public string OpCode { get; set; }

        [JsonProperty("params")]
        public BodyType Params { get; set; }
    }
}
