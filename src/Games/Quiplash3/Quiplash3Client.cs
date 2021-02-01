using System;
using JackboxGPT3.Games.Common;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Games.Quiplash3.Models;
using JackboxGPT3.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace JackboxGPT3.Games.Quiplash3
{
    public class Quiplash3Client : BaseJackboxClient<Quiplash3Room, Quiplash3Player>
    {
        private const string KEY_ROOM = "room";
        private const string KEY_PLAYER_PREFIX = "player:";
        
        private const string OP_OBJECT = "object";

        public event EventHandler<Revision<Quiplash3Room>> OnRoomUpdate;
        public event EventHandler<Revision<Quiplash3Player>> OnSelfUpdate;
        
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

        protected override void ServerMessageReceived(ServerMessage<JRaw> msg)
        {
            switch(msg.OpCode)
            {
                case OP_OBJECT:
                    var op = JsonConvert.DeserializeObject<ObjectOperation>(msg.Result.ToString());
                    HandleObjectOperation(op);
                    break;
            }
        }
        
        private void HandleObjectOperation(ObjectOperation op)
        {
            // special case: player ID
            if (op.Key == $"{KEY_PLAYER_PREFIX}{_gameState.PlayerId}")
            {
                var self = JsonConvert.DeserializeObject<Quiplash3Player>(op.Value.ToString());
                OnSelfUpdate?.Invoke(this, new Revision<Quiplash3Player>(_gameState.Self, self));
                _gameState.Self = self;
                return;
            }

            switch (op.Key)
            {
                case KEY_ROOM:
                    var room = JsonConvert.DeserializeObject<Quiplash3Room>(op.Value.ToString());
                    OnRoomUpdate?.Invoke(this, new Revision<Quiplash3Room>(_gameState.Room, room));
                    _gameState.Room = room;
                    break;
            }
        }
    }
}
