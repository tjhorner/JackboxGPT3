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
        
        protected override string KEY_ROOM => "room";
        protected override string KEY_PLAYER_PREFIX => "player:";
    }
}