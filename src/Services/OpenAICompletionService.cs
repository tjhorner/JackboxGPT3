using System;
using System.Collections.Generic;
using System.Linq;
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
        public OpenAICompletionService(IConfigurationProvider configuration) : this(Environment.GetEnvironmentVariable("OPENAI_KEY"), configuration) { }

        private OpenAICompletionService(string apiKey, IConfigurationProvider configuration)
        {
            _api = new OpenAIAPI(apiKey, configuration.OpenAIEngine);
        }

        public async Task<CompletionResponse> CompletePrompt(
            string prompt,
            CompletionParameters completionParameters,
            Func<CompletionResponse, bool> conditions = null,
            int maxTries = 5,
            string defaultResponse = ""
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

            if (!validResponse)
                result = new CompletionResponse
                {
                    FinishReason = "no_valid_responses",
                    Text = defaultResponse
                };

            return result;
        }
        
        public async Task<T> CompletePrompt<T>(
            string prompt,
            CompletionParameters completionParameters,
            Func<CompletionResponse, T> process,
            T defaultResponse,
            Func<T, bool> conditions = null,
            int maxTries = 5
        ) {
            var processedResult = defaultResponse;
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

                var result = ChoiceToCompletionResponse(apiResult.Completions[0]);
                processedResult = process(result);

                if (conditions == null) break;
                validResponse = conditions(processedResult);
            }

            return processedResult;
        }

        private static CompletionResponse ChoiceToCompletionResponse(Choice choice)
        {
            return new()
            {
                Text = choice.Text,
                FinishReason = choice.FinishReason
            };
        }
        
        public async Task<List<ICompletionService.SearchResponse>> SemanticSearch(string query, IList<string> documents)
        {
            var apiResults = await _api.Search.GetSearchResultsAsync(query, documents.ToArray());
            return apiResults.Select(apiResult => new ICompletionService.SearchResponse {Index = documents.IndexOf(apiResult.Key), Score = apiResult.Value}).ToList();
        }
    }
}
