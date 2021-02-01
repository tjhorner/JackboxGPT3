using System;
using JackboxGPT3.Games.Common;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Games.Fibbage3.Models;
using JackboxGPT3.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace JackboxGPT3.Games.Fibbage3
{
    public class Fibbage3Client : BaseJackboxClient<Fibbage3Room, Fibbage3Player>
    {
        private const string OP_CLIENT_SEND = "client/send";
        private const string OP_TEXT = "text";

        private const string KEY_ROOM = "bc:room";
        private const string KEY_PLAYER_PREFIX = "bc:customer:";

        public event EventHandler<Fibbage3Room> OnRoomUpdate;
        public event EventHandler<Fibbage3Player> OnSelfUpdate;

        public Fibbage3Client(IConfigurationProvider configuration, ILogger logger) : base(configuration, logger) {
            MessageReceived += ServerMessageReceived;
        }

        public void ChooseCategory(int index)
        {
            var operation = new ClientSendOperation<ChooseRequest<int>>
            {
                From = _gameState.PlayerId,
                To = 1,
                Body = new ChooseRequest<int> { Choice = index }
            };

            WsSend(OP_CLIENT_SEND, operation);
        }

        public void SubmitLie(string lie)
        {
            var operation = new ClientSendOperation<SendEntryRequest>
            {
                From = _gameState.PlayerId,
                To = 1,
                Body = new SendEntryRequest { Entry = lie }
            };

            WsSend(OP_CLIENT_SEND, operation);
        }


        public void SubmitTruth(int index, string text)
        {
            var operation = new ClientSendOperation<ChooseRequest<TruthChoice>>
            {
                From = _gameState.PlayerId,
                To = 1,
                Body = new ChooseRequest<TruthChoice>
                {
                    Choice = new TruthChoice
                    {
                        Order = index,
                        Text = text
                    }
                }
            };

            WsSend(OP_CLIENT_SEND, operation);
        }

        private void ServerMessageReceived(object sender, ServerMessage<JRaw> msg)
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
                var self = JsonConvert.DeserializeObject<Fibbage3Player>(op.Value);
                _gameState.Self = self;
                OnSelfUpdate?.Invoke(this, self);
                return;
            }

            switch (op.Key)
            {
                case KEY_ROOM:
                    var room = JsonConvert.DeserializeObject<Fibbage3Room>(op.Value);
                    _gameState.Room = room;
                    OnRoomUpdate?.Invoke(this, room);
                    break;
            }
        }
    }
}
