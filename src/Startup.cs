using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using JackboxGPT3.Engines;
using JackboxGPT3.Games.BlatherRound;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Games.Fibbage3;
using JackboxGPT3.Games.Quiplash3;
using JackboxGPT3.Games.SurviveTheInternet;
using JackboxGPT3.Games.WordSpud;
using JackboxGPT3.Services;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;

namespace JackboxGPT3
{
    public static class Startup
    {
        private static readonly HttpClient _httpClient = new();

        public static async Task Bootstrap(IConfigurationProvider configuration)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Is(Enum.Parse<LogEventLevel>(configuration.LogLevel, true))
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger = logger;

            var builder = new ContainerBuilder();
            builder.RegisterInstance(configuration).As<IConfigurationProvider>();
            builder.RegisterType<OpenAICompletionService>().As<ICompletionService>();
            builder.RegisterInstance<ILogger>(logger).SingleInstance();

            builder.RegisterGameEngines();

            var container = builder.Build();

            logger.Information("Starting up...");

            var roomCode = configuration.RoomCode;
            var ecastHost = configuration.EcastHost;

            logger.Debug($"Ecast host: {ecastHost}");
            logger.Information($"Trying to join room with code: {roomCode}");

            var response = await _httpClient.GetAsync($"https://{ecastHost}/api/v2/rooms/{roomCode}");

            try
            {
                response.EnsureSuccessStatusCode();
            } catch(HttpRequestException ex)
            {
                if (ex.StatusCode != HttpStatusCode.NotFound)
                    throw;

                logger.Error("Room not found.");
                return;
            }

            var roomResponse = JsonConvert.DeserializeObject<GetRoomResponse>(await response.Content.ReadAsStringAsync());
            var tag = roomResponse.Room.AppTag;

            if (!container.IsRegisteredWithKey<IJackboxEngine>(tag))
            {
                logger.Error($"Unsupported game: {tag}");
                return;
            }

            logger.Information($"Room found! Starting up {tag} engine...");
            container.ResolveNamed<IJackboxEngine>(tag);
        }

        private static void RegisterGameEngines(this ContainerBuilder builder)
        {
            // Game engines, keyed with appTag
            builder.RegisterType<Fibbage3Client>();
            builder.RegisterType<Fibbage3Engine>().Keyed<IJackboxEngine>("fibbage3");

            builder.RegisterType<Quiplash3Client>();
            builder.RegisterType<Quiplash3Engine>().Keyed<IJackboxEngine>("quiplash3");
            
            builder.RegisterType<WordSpudClient>();
            builder.RegisterType<WordSpudEngine>().Keyed<IJackboxEngine>("wordspud");
            
            builder.RegisterType<SurviveTheInternetClient>();
            builder.RegisterType<SurviveTheInternetEngine>().Keyed<IJackboxEngine>("survivetheinternet");
            
            builder.RegisterType<BlatherRoundClient>();
            builder.RegisterType<BlatherRoundEngine>().Keyed<IJackboxEngine>("blanky-blank");
        }
    }
}
