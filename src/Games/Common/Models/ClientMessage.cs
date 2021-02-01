using Newtonsoft.Json;

namespace JackboxGPT3.Games.Common.Models
{
    public struct ClientMessageOperation<TBody>
    {
        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("opcode")]
        public string OpCode { get; set; }

        [JsonProperty("params")]
        public TBody Params { get; set; }
    }
}
