using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Engines
{
    public abstract class BaseJackboxEngine : IJackboxEngine
    {
        protected abstract string Tag { get; }

        protected readonly ICompletionService CompletionService;
        
        private readonly ILogger _logger;

        protected BaseJackboxEngine(ICompletionService completionService, ILogger logger)
        {
            CompletionService = completionService;
            _logger = logger;
        }

        protected void LogWarning(string text)
        {
            _logger.Warning($"[{Tag}] {text}");
        }

        protected void LogError(string text)
        {
            _logger.Error($"[{Tag}] {text}");
        }

        protected void LogDebug(string text)
        {
            _logger.Debug($"[{Tag}] {text}");
        }

        protected void LogVerbose(string text)
        {
            _logger.Verbose($"[{Tag}] {text}");
        }

        protected void LogInfo(string text)
        {
            _logger.Information($"[{Tag}] {text}");
        }
    }
}
