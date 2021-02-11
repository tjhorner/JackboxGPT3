using JackboxGPT3.Games.Common;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Games.Quiplash3.Models;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Games.Quiplash3
{
    public class Quiplash3Client : PlayerSerializedClient<Quiplash3Room, Quiplash3Player>
    {
        public Quiplash3Client(IConfigurationProvider configuration, ILogger logger) : base(configuration, logger) { }

        public void ChooseCharacter(string name)
        {
            var req = new ChooseAvatarRequest(name);
            ClientSend(req);
        }
        
        public void SubmitQuip(string quip)
        {
            var req = new TextUpdateRequest(_gameState.Self.TextKey, quip);
            WsSend(TextUpdateRequest.OpCode, req);
        }

        public void ChooseFavorite(int index)
        {
            var req = new ChooseRequest<int>(index);
            ClientSend(req);
        }
    }
}
