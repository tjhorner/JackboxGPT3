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
        
        protected override string KEY_ROOM => "bc:room";
        protected override string KEY_PLAYER_PREFIX => "bc:customer:";
    }
}