using System;
using System.Threading.Tasks;
using OpenAI_API;
using static JackboxGPT3.Services.ICompletionService;

namespace JackboxGPT3.Services
{
    // ReSharper disable once InconsistentNaming
    public class OpenAICompletionService : ICompletionService
    {
        private readonly OpenAIAPI _api;

        /// <summary>
        /// Instantiate an <see cref="OpenAICompletionService"/> from the environment.
        /// </summary>
        public OpenAICompletionService() : this(Environment.GetEnvironmentVariable("OPENAI_KEY")) { }

        public OpenAICompletionService(string apiKey)
        {
            _api = new OpenAIAPI(apiKey, Engine.Davinci);
        }

        public async Task<CompletionResponse> CompletePrompt(
            string prompt,
            CompletionParameters completionParameters,
            Func<CompletionResponse, bool> conditions = null,
            int maxTries = 5
        ) {
            var result = new CompletionResponse();
            var validResponse = false;
            var tries = 0;

            while(!validResponse && tries < maxTries)
            {
                tries++;
                var apiResult = await _api.Completions.CreateCompletionAsync(
                    prompt,
                    completionParameters.MaxTokens,
                    completionParameters.Temperature,
                    completionParameters.TopP,
                    1,
                    logProbs: completionParameters.LogProbs,
                    echo: completionParameters.Echo,
                    presencePenalty: completionParameters.PresencePenalty,
                    frequencyPenalty: completionParameters.FrequencyPenalty,
                    stopSequences: completionParameters.StopSequences
                );

                result = ChoiceToCompletionResponse(apiResult.Completions[0]);

                if (conditions == null) break;
                validResponse = conditions(result);
            }

            return result;
        }

        private static CompletionResponse ChoiceToCompletionResponse(Choice choice)
        {
            return new()
            {
                Text = choice.Text,
                FinishReason = choice.FinishReason
            };
        }
    }
}
