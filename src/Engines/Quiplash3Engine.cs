using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Engines
{
    public class Quiplash3Engine : BaseJackboxEngine
    {
        protected override string Tag => "quiplash3";

        public Quiplash3Engine(ICompletionService completionService, ILogger logger) : base(completionService, logger) { }
    }
}
