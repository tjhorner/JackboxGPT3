using Newtonsoft.Json;

namespace JackboxGPT3.Games.Common.Models
{
    public struct ServerMessage<TBody>
    {
        [JsonProperty("pc")]
        public int Seq { get; set; }

        [JsonProperty("opcode")]
        public string OpCode { get; set; }

        [JsonProperty("result")]
        public TBody Result { get; set; }
    }
}
