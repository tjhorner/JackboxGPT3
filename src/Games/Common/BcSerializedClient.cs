using System;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace JackboxGPT3.Games.Common
{
    /// <summary>
    /// A Jackbox client for games which use the "bc:" serialization format. It's
    /// named this way because the keys sent by the game are prefixed with "bc:".
    /// I have no idea what "bc" stands for but it seems several games share this
    /// serialization format. 
    /// </summary>
    public abstract class BcSerializedClient<TRoom, TPlayer> : BaseJackboxClient<TRoom, TPlayer>
    {
        protected BcSerializedClient(IConfigurationProvider configuration, ILogger logger) : base(configuration, logger)  {  }
        
        private const string KEY_ROOM = "bc:room";
        private const string KEY_PLAYER_PREFIX = "bc:customer:";
        
        private const string OP_TEXT = "text";
        
        public event EventHandler<Revision<TRoom>> OnRoomUpdate;
        public event EventHandler<Revision<TPlayer>> OnSelfUpdate;
        
        protected override void ServerMessageReceived(ServerMessage<JRaw> msg)
        {
            switch(msg.OpCode)
            {
                case OP_TEXT:
                    var op = JsonConvert.DeserializeObject<TextOperation>(msg.Result.ToString());
                    HandleTextOperation(op);
                    break;
            }
        }
        
        private void HandleTextOperation(TextOperation op)
        {
            // special case: player ID
            if (op.Key == $"{KEY_PLAYER_PREFIX}{PlayerId}")
            {
                var self = JsonConvert.DeserializeObject<TPlayer>(op.Value);
                OnSelfUpdate?.Invoke(this, new Revision<TPlayer>(_gameState.Self, self));
                _gameState.Self = self;
                return;
            }

            switch (op.Key)
            {
                case KEY_ROOM:
                    var room = JsonConvert.DeserializeObject<TRoom>(op.Value);
                    OnRoomUpdate?.Invoke(this, new Revision<TRoom>(_gameState.Room, room));
                    _gameState.Room = room;
                    break;
            }
        }
    }
}