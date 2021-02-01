using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JackboxGPT3.Extensions;
using JackboxGPT3.Games.Fibbage3;
using JackboxGPT3.Games.Fibbage3.Models;
using JackboxGPT3.Services;
using Serilog;
using static JackboxGPT3.Services.ICompletionService;

namespace JackboxGPT3.Engines
{
    public class Fibbage3Engine : BaseJackboxEngine<Fibbage3Client>
    {
        protected override string Tag => "fibbage3";

        private bool _lieLock;
        private bool _truthLock;

        public Fibbage3Engine(ICompletionService completionService, ILogger logger, Fibbage3Client client)
            : base(completionService, logger, client)
        {
            JackboxClient.OnRoomUpdate += OnRoomUpdate;
            JackboxClient.OnSelfUpdate += OnSelfUpdate;
            JackboxClient.Connect();
        }

        private void OnSelfUpdate(object sender, Fibbage3Player self)
        {
            if (JackboxClient.GameState.Room.State == RoomState.EndShortie || self.Error != null)
                _lieLock = _truthLock = false;

            if (JackboxClient.GameState.Room.State == RoomState.CategorySelection && self.IsChoosing)
                ChooseRandomCategory();

            if (JackboxClient.GameState.Room.State == RoomState.EnterText && !_lieLock)
                SubmitLie();

            if (JackboxClient.GameState.Room.State == RoomState.ChooseLie && !_truthLock)
                SubmitTruth();
        }

        private void OnRoomUpdate(object sender, Fibbage3Room room)
        {
            LogDebug($"New room state: {room.State}");
        }
        
        #region Game Actions
        private async void SubmitLie()
        {
            _lieLock = true;

            var prompt = CleanPromptForEntry(JackboxClient.GameState.Self.Question);
            LogInfo($"Asking GPT-3 for lie in response to \"{prompt}\".");

            var lie = await ProvideLie(prompt);
            LogInfo($"Submitting lie \"{lie}\".");

            JackboxClient.SubmitLie(lie);
        }

        private async void SubmitTruth()
        {
            _truthLock = true;

            var prompt = CleanPromptForEntry(JackboxClient.GameState.Room.Question);
            LogInfo("Asking GPT-3 to choose truth.");

            var choices = JackboxClient.GameState.Self.LieChoices;
            var truth = await ProvideTruth(prompt, choices);
            LogInfo($"Submitting truth {truth}.");

            JackboxClient.SubmitTruth(truth, choices[truth].Text);
        }

        private async void ChooseRandomCategory()
        {
            var room = JackboxClient.GameState.Room;

            LogInfo("Time to choose a category.");
            await Task.Delay(3000);

            var choices = room.CategoryChoices;
            var category = choices.RandomIndex();
            LogInfo($"Choosing category \"{choices[category].Trim()}\".");

            JackboxClient.ChooseCategory(category);
        }
        #endregion

        #region GPT-3 Prompts
        private async Task<string> ProvideLie(string fibPrompt)
        {
            var prompt = $"Here are some prompts from the game Fibbage, in which players attempt to write convincing lies to trick others.\n\nQ: In the mid-1800s, Queen Victoria employed a man named Jack Black, whose official job title was Royal _______.\nA: Flute player\n\nQ: In 2016, KFC announced it created a _______ that smells like fried chicken.\nA: Scratch 'n' sniff menu\n\nQ: Due to a habit he had while roaming the halls of the White House, President Lyndon B. Johnson earned the nickname \"_______ Johnson.\"\nA: Desk Butt\n\nQ: {fibPrompt}\nA:";

            var result = await CompletionService.CompletePrompt(prompt, new CompletionParameters
            {
                Temperature = 0.7,
                MaxTokens = 16,
                TopP = 1,
                FrequencyPenalty = 0.2,
                StopSequences = new[] { "\n" }
            }, completion => !completion.Text.Contains("___") && completion.Text.Length <= 45);

            return result.Text.Trim();
        }

        private async Task<int> ProvideTruth(string fibPrompt, IReadOnlyList<LieChoice> lies)
        {
            var options = "";

            for(var i = 0; i < lies.Count; i++)
            {
                options += $"{i + 1}. {lies[i].Text}\n";
            }

            var prompt = $"I was given a list of lies and one truth for the prompt \"${fibPrompt}\". These were my options:\n\n${options}\nI think the truth is answer number";

            var result = await CompletionService.CompletePrompt(prompt, new CompletionParameters
            {
                Temperature = 1,
                MaxTokens = 1,
                TopP = 1,
                StopSequences = new[] { "\n" }
            }, completion =>
            {
                try
                {
                    _ = int.Parse(completion.Text.Trim());
                } catch(FormatException)
                {
                    return false;
                }

                return completion.Text.Split("|").Length <= lies.Count;
            });

            return int.Parse(result.Text.Trim()) - 1;
        }
        #endregion

        #region Prompt Cleanup

        private static string CleanPromptForEntry(string prompt)
        {
            return prompt.StripHtml();
        }
        #endregion
    }
}
