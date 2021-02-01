using JackboxGPT3.Games.Quiplash3;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Engines
{
    public class Quiplash3Engine : BaseJackboxEngine<Quiplash3Client>
    {
        protected override string Tag => "quiplash3";

        public Quiplash3Engine(ICompletionService completionService, ILogger logger, Quiplash3Client client)
            : base(completionService, logger, client)
        {
            client.Connect();
        }
    }
}
