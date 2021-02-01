using JackboxGPT3.Games.Common;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Games.Quiplash3.Models;
using JackboxGPT3.Services;
using Newtonsoft.Json.Linq;
using Serilog;

namespace JackboxGPT3.Games.Quiplash3
{
    public class Quiplash3Client : BaseJackboxClient<Quiplash3Room, Quiplash3Player>
    {
        public Quiplash3Client(IConfigurationProvider configuration, ILogger logger) : base(configuration, logger) { }
        
        protected override void ServerMessageReceived(ServerMessage<JRaw> message)
        {
            // no-op
        }
    }
}
