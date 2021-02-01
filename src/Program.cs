using Autofac;
using dotenv.net;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3
{
    public static class Program
    {
        public static void Main()
        {
            DotEnv.AutoConfig();

            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger = logger;

            var builder = new ContainerBuilder();
            builder.RegisterType<CommandLineConfigurationProvider>().As<IConfigurationProvider>();
            builder.RegisterType<OpenAICompletionService>().As<ICompletionService>();
            builder.RegisterInstance<ILogger>(logger).SingleInstance();

            builder.RegisterGameEngines();

            var container = builder.Build();
            Startup.Bootstrap(container).Wait();
        }
    }
}
