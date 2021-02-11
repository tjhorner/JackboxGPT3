using System;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace JackboxGPT3.Games.Common
{
    public abstract class PlayerSerializedClient<TRoom, TPlayer> : BaseJackboxClient<TRoom, TPlayer>
    {
        protected PlayerSerializedClient(IConfigurationProvider configuration, ILogger logger) : base(configuration, logger)  {  }
        
        private const string KEY_ROOM = "room";
        private const string KEY_PLAYER_PREFIX = "player:";
        
        private const string OP_OBJECT = "object";
        
        public event EventHandler<Revision<TRoom>> OnRoomUpdate;
        public event EventHandler<Revision<TPlayer>> OnSelfUpdate;
        
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
        
        protected virtual void HandleObjectOperation(ObjectOperation op)
        {
            // special case: player ID
            if (op.Key == $"{KEY_PLAYER_PREFIX}{_gameState.PlayerId}")
            {
                var self = JsonConvert.DeserializeObject<TPlayer>(op.Value.ToString());
                OnSelfUpdate?.Invoke(this, new Revision<TPlayer>(_gameState.Self, self));
                _gameState.Self = self;
                return;
            }

            switch (op.Key)
            {
                case KEY_ROOM:
                    var room = JsonConvert.DeserializeObject<TRoom>(op.Value.ToString());
                    OnRoomUpdate?.Invoke(this, new Revision<TRoom>(_gameState.Room, room));
                    _gameState.Room = room;
                    break;
            }
        }
    }
}