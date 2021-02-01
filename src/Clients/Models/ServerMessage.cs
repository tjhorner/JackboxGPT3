using Newtonsoft.Json;

namespace JackboxGPT3.Clients.Models
{
    public struct ServerMessage<BodyType>
    {
        [JsonProperty("pc")]
        public int Seq { get; set; }

        [JsonProperty("opcode")]
        public string OpCode { get; set; }

        [JsonProperty("result")]
        public BodyType Result { get; set; }
    }
}
