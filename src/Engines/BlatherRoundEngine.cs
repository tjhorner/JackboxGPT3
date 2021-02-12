using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JackboxGPT3.Extensions;
using JackboxGPT3.Games.BlatherRound;
using JackboxGPT3.Games.BlatherRound.Models;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Engines
{
    public class BlatherRoundEngine : BaseJackboxEngine<BlatherRoundClient>
    {
        protected override string Tag => "blanky-blank";

        private readonly Random _random = new();
        private readonly List<string> _guessesUsedThisRound = new();
        private bool _writing = false;
        
        public BlatherRoundEngine(ICompletionService completionService, ILogger logger, BlatherRoundClient client) : base(completionService, logger, client)
        {
            JackboxClient.OnSelfUpdate += OnSelfUpdate;
            JackboxClient.OnWriteNewSentence += OnWriteNewSentence;
            JackboxClient.OnPlayerStartedPresenting += OnPlayerStartedPresenting;
            JackboxClient.Connect();
        }

        private void OnPlayerStartedPresenting(object sender, EventArgs e)
        {
            _guessesUsedThisRound.Clear();
        }

        private void OnSelfUpdate(object sender, Revision<BlatherRoundPlayer> revision)
        {
            if(revision.Old.State != revision.New.State && revision.New.State == PlayerState.MakeSingleChoice)
                ChoosePassword(revision.New);
            else if (revision.New.State == PlayerState.EnterSingleText && revision.Old.EntryId != revision.New.EntryId)
                SubmitGuess();
        }

        private async void SubmitGuess()
        {
            await Task.Delay(5000);
            
            var guess = await ProvideGuess();
            _guessesUsedThisRound.Add(guess);
            
            JackboxClient.SubmitGuess(guess);
        }

        private void ChoosePassword(BlatherRoundPlayer self)
        {
            _guessesUsedThisRound.Clear();
            JackboxClient.ChoosePassword(self.Choices.Where(c => c.ClassName != "refresh").ToList().RandomIndex());
        }

        private async void OnWriteNewSentence(object sender, Sentence sentence)
        {
            if (_writing) return;
            await Task.Delay(10000);

            _writing = true;
            
            while (true)
            {
                var sentenceResult = await WriteSentence();

                if (sentenceResult == SentenceResult.Skip)
                {
                    JackboxClient.SubmitSentence(true);
                    await Task.Delay(1000);
                }
                else if (sentenceResult == SentenceResult.Submit)
                {
                    JackboxClient.SubmitSentence();
                    break;
                }
                else if (sentenceResult == SentenceResult.DoNothing)
                {
                    break;
                }
            }

            _writing = false;
        }

        private async Task<SentenceResult> WriteSentence()
        {
            if (JackboxClient.CurrentSentence == null || JackboxClient.GameState.Self.Prompt.Html == null) return SentenceResult.DoNothing;

            var parts = JackboxClient.CurrentSentence.Parts;
            var prompt = GetCleanPrompt();
            
            LogVerbose($"Prompt is {prompt}.");

            if (JackboxClient.CurrentSentence.Type != SentenceType.Response)
            {
                // Randomly decide which order to select the parts in
                if (_random.Next(0, 2) == 0)
                {
                    LogVerbose("Processing parts from last to first.");
                    for (var index = parts.Count - 1; index >= 0; index--)
                        if (!await ProcessPart(prompt, parts, index, PartOrder.Before))
                            return SentenceResult.Skip;
                }
                else
                {
                    LogVerbose("Processing parts from first to last.");
                    for (var index = 0; index < parts.Count; index++)
                        if (!await ProcessPart(prompt, parts, index))
                            return SentenceResult.Skip;
                }
            }
            else
            {
                var unusedChoices = parts[1].Choices.Where(choice => !_guessesUsedThisRound.Contains(choice)).ToList();

                if (unusedChoices.Count == 0)
                    return SentenceResult.Skip;

                var results = await CompletionService.SemanticSearch(prompt, unusedChoices);
                results.Sort((a, b) => (int)(b.Score - a.Score));
                
                foreach (var result in results)
                    LogVerbose($"{unusedChoices[result.Index]}: {result.Score}");
                
                // Randomly decide whether to check best or worst performers
                if (_random.Next(0, 2) == 0 && results[0].Score >= 50)
                {
                    var qualifierIndex = -1;
                    foreach (var qualifier in Sentence.PHRASES_SIMILAR_TO)
                    {
                        qualifierIndex = parts[0].Choices.IndexOf(qualifier);
                        if (qualifierIndex != -1) break;
                    }
                    
                    JackboxClient.ChooseWord(0, qualifierIndex);
                    await Task.Delay(100);
                    
                    _guessesUsedThisRound.Add(unusedChoices[results[0].Index]);
                    
                    JackboxClient.ChooseWord(1, parts[1].Choices.IndexOf(unusedChoices[results[0].Index]));
                }
                else if(results.Last().Score <= -20)
                {
                    var qualifierIndex = -1;
                    foreach (var qualifier in Sentence.PHRASES_DISSIMILAR_FROM)
                    {
                        qualifierIndex = parts[0].Choices.IndexOf(qualifier);
                        if (qualifierIndex != -1) break;
                    }
                    
                    JackboxClient.ChooseWord(0, qualifierIndex);
                    await Task.Delay(100);
                    
                    _guessesUsedThisRound.Add(unusedChoices[results.Last().Index]);
                    
                    JackboxClient.ChooseWord(1, parts[1].Choices.IndexOf(unusedChoices[results.Last().Index]));
                }
                else
                    return SentenceResult.Skip;

                await Task.Delay(100);
            }

            return SentenceResult.Submit;
        }

        private async Task<bool> ProcessPart(string prompt, IList<SentencePart> parts, int index, PartOrder order = PartOrder.After)
        {
            var part = parts[index];
            if (!part.ShouldChoose) return true;
            
            var chosenWords = string.Join(' ', JackboxClient.CurrentSentence.ChosenWords);
            
            LogVerbose($"chosenWords: {chosenWords}");

            var newChoices =
                order == PartOrder.After ?
                    part.Choices.Select(c => $"{chosenWords} {c}".Trim()).ToList() :
                    part.Choices.Select(c => $"{c} {chosenWords}".Trim()).ToList();
        
            var results = await CompletionService.SemanticSearch(prompt, newChoices);
            
            results.Sort((a, b) => (int)(b.Score - a.Score));

            foreach (var result in results)
                LogVerbose($"{newChoices[result.Index]}: {result.Score}");
        
            // Top performers are the results within 20 points of the top performing result.
            // Will be chosen randomly.
            var topPerformers = results.Where(r => results[0].Score - r.Score <= 20).ToList();
            
            if (JackboxClient.CurrentSentence.Type != SentenceType.Writing && chosenWords == "" && results[0].Score < 10)
            {
                // not confident enough; skip
                return false;
            }

            // choose from top 3
            var chosenWord = topPerformers[topPerformers.RandomIndex()].Index;
            JackboxClient.ChooseWord(index, chosenWord);
                
            await Task.Delay(500);
            return true;
        }

        private async Task<string> ProvideGuess()
        {
            /*
I was given a list of sentences to describe a thing:

It's a neat-o rectangle object.
It is the gadget.
Talk about silvery!
It has the knob.
Oh, line art!
Quite simply, it's a silvery art.
Talk about plasticky!
It makes the art.
Oh, childhood!

My guess: Etch-a-Sketch
###
             */
            
            var prompt =
                $@"A list of sentences to describe a place:

It's a fantastic food place.
It's where you have the guilty pleasure.
So much din-din!
It's where you delight in the wrap.
It's a spicy food.

Guess: Taco Bell.
###
A list of sentences to describe a {JackboxClient.CurrentCategory}:

{string.Join('\n', JackboxClient.CurrentSentences)}

Guess:";
            
            LogVerbose($"GPT-3 Prompt: {prompt}");
            
            var result = await CompletionService.CompletePrompt(prompt, new ICompletionService.CompletionParameters
                {
                    Temperature = 0.8,
                    MaxTokens = 16,
                    TopP = 1,
                    FrequencyPenalty = 0.3,
                    PresencePenalty = 0.2,
                    StopSequences = new[] { "\n", "###", "." }
                }, completion => CleanAnswer(completion.Text) != "" && CleanAnswer(completion.Text).Length <= 40 && !_guessesUsedThisRound.Contains(CleanAnswer(completion.Text)),
                defaultResponse: "");

            return CleanAnswer(result.Text);
        }

        private string GetCleanPrompt()
        {
            return JackboxClient.GameState.Self.Prompt.Html.Replace("Describe ", "");
        }

        private static string CleanAnswer(string answer)
        {
            answer = answer.Trim();
            var articlesRegex = new Regex("^(a|an|the) ", RegexOptions.IgnoreCase);
            answer = articlesRegex.Replace(answer, "").Trim();
            return answer.TrimEnd('.').TrimQuotes();
        }
    }

    internal enum SentenceResult
    {
        Skip,
        Submit,
        DoNothing
    }

    internal enum PartOrder
    {
        Before,
        After
    }
}