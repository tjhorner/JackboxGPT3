using JackboxGPT3.Games.Common;
using JackboxGPT3.Games.SurviveTheInternet.Models;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Games.SurviveTheInternet
{
    public class SurviveTheInternetClient : BcSerializedClient<SurviveTheInternetRoom, SurviveTheInternetPlayer>
    {
        public SurviveTheInternetClient(IConfigurationProvider configuration, ILogger logger) : base(configuration, logger) { }
    }
}