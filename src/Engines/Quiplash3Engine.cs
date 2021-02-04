using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using JackboxGPT3.Extensions;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Games.Quiplash3;
using JackboxGPT3.Games.Quiplash3.Models;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Engines
{
    public class Quiplash3Engine : BaseJackboxEngine<Quiplash3Client>
    {
        protected override string Tag => "quiplash3";

        private bool _selectedAvatar;
        private readonly Random _random = new();

        public Quiplash3Engine(ICompletionService completionService, ILogger logger, Quiplash3Client client)
            : base(completionService, logger, client)
        {
            JackboxClient.OnRoomUpdate += OnRoomUpdate;
            JackboxClient.OnSelfUpdate += OnSelfUpdate;
            JackboxClient.Connect();
        }

        private void OnSelfUpdate(object sender, Revision<Quiplash3Player> revision)
        {
            switch (revision.New.State)
            {
                case RoomState.EnterSingleText:
                    SubmitQuip(revision.New);
                    break;
                case RoomState.EnterTextList when revision.New.Entries == false:
                    SubmitThrip(revision.New);
                    break;
                case RoomState.MakeSingleChoice when revision.New.ChoiceType == ChoiceType.ChoseQuip:
                    VoteQuip(revision.New);
                    break;
                case RoomState.MakeSingleChoice when revision.New.ChoiceType == ChoiceType.ChooseQuip:
                    VoteThrip(revision.New);
                    break;
            }
        }

        private void OnRoomUpdate(object sender, Revision<Quiplash3Room> room)
        {
            if (_selectedAvatar) return;
            
            _selectedAvatar = true;
            var availableChars = room.New.Characters.Where(c => c.Available).ToList();
            var charIndex = availableChars.RandomIndex();
            JackboxClient.ChooseCharacter(availableChars[charIndex].Name);
        }

        private async void VoteQuip(Quiplash3Player self)
        {
            var prompt = CleanPromptForSelection(self.Prompt.Html);
            if (prompt == "") return;
            
            LogInfo("Asking GPT-3 to choose favorite quip.");

            var quips = self.Choices.Select(choice => CleanQuipForSelection(choice.Html)).ToList();
            var favorite = await ProvideFavorite(prompt, quips);
            
            LogInfo($"Choosing \"{quips[favorite]}\".");
            
            JackboxClient.ChooseFavorite(favorite);
        }
        
        private async void VoteThrip(Quiplash3Player self)
        {
            var prompt = CleanPromptForSelection(self.Prompt.Html);
            if (prompt == "") return;
            
            LogInfo("Asking GPT-3 to choose favorite thrip.");

            var thrips = self.Choices.Select(choice => CleanThripForSelection(choice.Html)).ToList();
            var favorite = await ProvideFavorite(prompt, thrips);
            
            LogInfo($"Choosing \"{thrips[favorite]}\".");
            
            JackboxClient.ChooseFavorite(favorite);
        }

        private async void SubmitQuip(Quiplash3Player self)
        {
            if (self.Prompt.Html == null) return;
            var prompt = CleanPromptForEntry(self.Prompt.Html);
            if (prompt == "") return;
            
            LogInfo($"Asking GPT-3 for response to \"{prompt}\".");
            
            var quip = await ProvideQuip(prompt);
            LogInfo($"Responding with \"{quip}\".");
            
            JackboxClient.SubmitQuip(quip);
        }
        
        private async Task<string> ProvideQuip(string qlPrompt)
        {
            var prompt = $@"Below are some questions and outlandish, funny, ridiculous answers to them.

Q: Honestly, you can never have too many _______
Funny Answer: Reasons to stay inside

Q: You know a restaurant is bad when the waiter says ""_______""
Funny Answer: I don't know, but we can find out!

Q: What would you call your ANTI-social network?
Funny Answer: Slankbook

Q: Your fish are bored! You should put a _______ in their tank to amuse them.
Funny Answer: A shark

Q: {qlPrompt}
Funny Answer:";
            
            LogDebug($"GPT-3 Prompt: {prompt}");
            
            var result = await CompletionService.CompletePrompt(prompt, new ICompletionService.CompletionParameters
            {
                Temperature = 0.5,
                MaxTokens = 16,
                TopP = 1,
                FrequencyPenalty = 0.2,
                PresencePenalty = 0.1,
                StopSequences = new[] { "\n" }
            }, completion => !completion.Text.Contains("___") && completion.Text.Length <= 45,
                defaultResponse: "⁇");

            return result.Text.Trim().TrimEnd('.');
        }
        
        private async void SubmitThrip(Quiplash3Player self)
        {
            if (self.Prompt.Html == null) return;
            var prompt = CleanPromptForEntry(self.Prompt.Html);
            if (prompt == "") return;
            
            LogInfo($"Asking GPT-3 for response to \"{prompt}\".");
            
            var quip = await ProvideThrip(prompt);
            LogInfo($"Responding with \"{quip}\".");
            
            JackboxClient.SubmitQuip(quip);
        }
        
        private async Task<string> ProvideThrip(string qlPrompt)
        {
            var prompt = $@"In the third round of the game Quiplash, players must take a prompt and give three different answers that make sense, separated by a | character.

Q: Three things every good orgy has
Funny Answer: scented oils|a non-disclosure agreement|disgraced politician

Q: The three things you must do to survive a zombie apocalypse
Funny Answer: hunker down|play video games|hope this all blows over

Q: The only three things that can bring true happiness
Funny Answer: money|sex|a long hug

Q: {qlPrompt}
Funny Answer:";
            
            LogDebug($"GPT-3 Prompt: {prompt}");
            
            var result = await CompletionService.CompletePrompt(prompt, new ICompletionService.CompletionParameters
            {
                Temperature = 0.7,
                MaxTokens = 32,
                TopP = 1,
                FrequencyPenalty = 0.2,
                PresencePenalty = 0.1,
                StopSequences = new[] { "\n" }
            }, completion => completion.Text.Split("|").Length == 3 && !completion.Text.Contains("___") && completion.Text.Length <= 45,
                defaultResponse: "Oops|GPT-3 didn't work|Sigh");

            return string.Join("\n", result.Text.Split("|")).Trim();
        }
        
        private async Task<int> ProvideFavorite(string qlPrompt, IReadOnlyList<string> quips)
        {
            var options = "";

            for(var i = 0; i < quips.Count; i++)
                options += $"{i + 1}. {quips[i]}\n";

            var prompt = $"I was playing a game of Quiplash, and the prompt was \"${qlPrompt}\". These were my options:\n\n${options}\nThe funniest was prompt number";

            var result = await CompletionService.CompletePrompt(prompt, new ICompletionService.CompletionParameters
            {
                Temperature = 1,
                MaxTokens = 1,
                TopP = 1,
                StopSequences = new[] { "\n" }
            }, completion =>
            {
                try
                {
                    var answer = int.Parse(completion.Text.Trim());
                    return answer <= quips.Count && answer > 0;
                }
                catch(FormatException)
                {
                    return false;
                }
            }, defaultResponse: _random.Next(0, quips.Count).ToString());

            return int.Parse(result.Text.Trim()) - 1;
        }

        #region Prompt Cleanup
        public static string CleanPromptForEntry(string prompt)
        {
            prompt = prompt.ToLower();
            
            var doc = new XmlDocument();
            doc.LoadXml($"<root>{prompt}</root>");
            return doc.FirstChild?.ChildNodes[1]?.InnerText.Trim() ?? "";
        }

        public static string CleanPromptForSelection(string prompt)
        {
            prompt = prompt.ToLower();
            return Regex.Replace(prompt, "<br \\/>.+", string.Empty).Trim();
        }
        
        public static string CleanQuipForSelection(string quip)
        {
            quip = quip.ToLower();
            
            var doc = new XmlDocument();
            doc.LoadXml($"<root>{quip}</root>");
            return doc.InnerText.Trim();
        }
        
        public static string CleanThripForSelection(string thrip)
        {
            thrip = thrip.ToLower();
            
            var doc = new XmlDocument();
            doc.LoadXml($"<root>{thrip}</root>");
            
            var quips = doc.FirstChild?.ChildNodes
                .Cast<XmlNode>()
                .Select(node => node.InnerText);

            return string.Join("|", quips ?? throw new InvalidOperationException()).Trim();
        }
        #endregion
    }
}
