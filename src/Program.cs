using Autofac;
using CommandLine;
using dotenv.net;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            DotEnv.AutoConfig();
            Parser.Default.ParseArguments<CommandLineConfigurationProvider>(args)
                .WithParsed((conf) => Startup.Bootstrap(conf).Wait());
        }
    }
}
