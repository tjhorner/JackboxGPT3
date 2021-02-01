using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using JackboxGPT3.Extensions;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Websocket.Client;
using Websocket.Client.Models;

namespace JackboxGPT3.Games.Common
{
    public abstract class BaseJackboxClient<TRoom, TPlayer> : IJackboxClient
    {
        private const string OP_CLIENT_WELCOME = "client/welcome";

        public event EventHandler<ClientWelcome> PlayerStateChanged;

        /// <summary>
        /// Handle a raw <see cref="ServerMessage{Body}"/> event from ecast.
        /// You'll need to deserialize the <see cref="ServerMessage{Body}.Result"/> yourself
        /// depending on the opcode.
        /// </summary>
        protected abstract void ServerMessageReceived(ServerMessage<JRaw> message);

        private readonly IConfigurationProvider _configuration;
        private readonly ILogger _logger;

        protected Guid PlayerId = Guid.NewGuid();
        // ReSharper disable once InconsistentNaming
        protected GameState<TRoom, TPlayer> _gameState;

        private WebsocketClient _webSocket;
        private ManualResetEvent _exitEvent;
        private int _msgSeq;

        public GameState<TRoom, TPlayer> GameState => _gameState;

        protected BaseJackboxClient(IConfigurationProvider configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void Connect()
        {
            var bootstrap = new BootstrapPayload
            {
                Role = "player",
                Name = _configuration.PlayerName,
                UserId = PlayerId.ToString(),
                Format = "json",
                Password = ""
            };

            var url = new Uri($"wss://{_configuration.EcastHost}/api/v2/rooms/{_configuration.RoomCode}/play?{bootstrap.AsQueryString()}");

            _logger.Debug($"Trying to connect to ecast websocket with url: {url}");

            _webSocket = new WebsocketClient(url, () =>
            {
                var nativeClient = new ClientWebSocket();
                nativeClient.Options.AddSubProtocol("ecast-v0");
                return nativeClient;
            }) {
                MessageEncoding = Encoding.UTF8,
                IsReconnectionEnabled = false
            };

            _exitEvent = new ManualResetEvent(false);

            _webSocket.MessageReceived.Subscribe(WsReceived);
            _webSocket.ReconnectionHappened.Subscribe(WsConnected);
            _webSocket.DisconnectionHappened.Subscribe(WsDisconnected);

            _webSocket.Start();
            _exitEvent.WaitOne();
        }

        private void WsReceived(ResponseMessage msg)
        {
            var srvMsg = JsonConvert.DeserializeObject<ServerMessage<JRaw>>(msg.Text);

            if(srvMsg.OpCode == OP_CLIENT_WELCOME)
            {
                var cw = JsonConvert.DeserializeObject<ClientWelcome>(srvMsg.Result.ToString());
                HandleClientWelcome(cw);
                PlayerStateChanged?.Invoke(this, cw);
            }

            ServerMessageReceived(srvMsg);
        }

        private void WsConnected(ReconnectionInfo inf)
        {
            _logger.Information("Connected to Jackbox games services.");
        }

        private void WsDisconnected(DisconnectionInfo inf)
        {
            _logger.Information("Disconnected from Jackbox games services.");
            _exitEvent?.Set();
        }

        private void HandleClientWelcome(ClientWelcome cw)
        {
            _gameState.PlayerId = cw.Id;
            _logger.Debug($"Client welcome message received. Player ID: {_gameState.PlayerId}");
        }

        protected void WsSend<T>(string opCode, T body)
        {
            _msgSeq++;

            var clientMessage = new ClientMessageOperation<T>
            {
                Seq = _msgSeq,
                OpCode = opCode,
                Params = body
            };

            var msg = JsonConvert.SerializeObject(clientMessage);
            _webSocket.Send(msg);
        }
    }
}
