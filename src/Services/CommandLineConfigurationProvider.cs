using CommandLine;
// ReSharper disable ClassNeverInstantiated.Global

namespace JackboxGPT3.Services
{
    public class CommandLineConfigurationProvider : DefaultConfigurationProvider
    {
        [Value(0, Required = true, HelpText = "The room code to join.", MetaName = "room-code")]
        public override string RoomCode { get; set; }

        [Option("engine", Default = "davinci", HelpText =
            "The GPT-3 model to use for completions. Some game engines may use different prompts. " +
            "For example, the instruct-series models might be given a more direct prompt with no examples. " +
            "For a list of engines and the differences between them, check out OpenAI's docs: " +
            "https://beta.openai.com/docs/engines"
        )]
        public override string OpenAIEngine { get; set; }

        [Option("verbosity", Default = "information", HelpText = "Log level to output. Possible values: verbose, debug, information, warning, error, fatal")]
        public override string LogLevel { get; set; }
    }
}
