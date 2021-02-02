using JackboxGPT3.Games.Common;
using JackboxGPT3.Games.WordSpud.Models;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Games.WordSpud
{
    public class WordSpudClient : BcSerializedClient<WordSpudRoom, WordSpudPlayer>
    {
        public WordSpudClient(IConfigurationProvider configuration, ILogger logger) : base(configuration, logger) { }

        public void SubmitSpud(string spud)
        {
            var req = new SubmitSpudRequest(spud);
            ClientSend(req);
        }
        
        public void Vote(int vote)
        {
            var req = new VoteRequest(vote);
            ClientSend(req);
        }
    }
}