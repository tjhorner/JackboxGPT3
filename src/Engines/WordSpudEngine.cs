using System.Linq;
using System.Threading.Tasks;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Games.WordSpud;
using JackboxGPT3.Games.WordSpud.Models;
using JackboxGPT3.Services;
using Serilog;
using static JackboxGPT3.Services.ICompletionService;

namespace JackboxGPT3.Engines
{
    public class WordSpudEngine : BaseJackboxEngine<WordSpudClient>
    {
        protected override string Tag => "wordspud";
        
        public WordSpudEngine(ICompletionService completionService, ILogger logger, WordSpudClient client) : base(completionService, logger, client)
        {
            JackboxClient.OnSelfUpdate += OnSelfUpdate;
            JackboxClient.OnRoomUpdate += OnRoomUpdate;
            JackboxClient.Connect();
        }

        private void OnRoomUpdate(object sender, Revision<WordSpudRoom> revision)
        {
            if (revision.Old.State != revision.New.State &&
                revision.New.State == RoomState.Vote)
                VoteSpud();
        }

        private void OnSelfUpdate(object sender, Revision<WordSpudPlayer> revision)
        {
            if (revision.New.State == RoomState.GameplayEnter &&
                (JackboxClient.GameState.Room.Spud != null && JackboxClient.GameState.Room.Spud == ""))
                SubmitSpud();
        }

        private async void SubmitSpud()
        {
            var currentWord = JackboxClient.GameState.Room.CurrentWord.Trim().Split(" ").Last();
            LogInfo($"Getting a spud for \"{currentWord}\".");

            var spud = await ProvideSpud(currentWord);
            LogInfo($"Submitting \"{spud}\".");
            
            JackboxClient.SubmitSpud(spud);
        }
        
        private async void VoteSpud()
        {
            if (JackboxClient.GameState.Self.State == RoomState.GameplayEnter) return;
            LogInfo("Voting.");
            
            await Task.Delay(1000);
            JackboxClient.Vote(1);
        }

        private async Task<string> ProvideSpud(string currentWord)
        {
            var prompt = $@"The game Word Spud is played by continuing a word or phrase with a funny related word or phrase. For example:

- jellyfish
- deal with it
- fishsticks
- beat saber
- tailor-made
- real life
- how do you do
- {currentWord}";

            LogDebug($"GPT-3 Prompt: {prompt}");

            var result = await CompletionService.CompletePrompt(prompt, new CompletionParameters
            {
                Temperature = 0.8,
                MaxTokens = 16,
                TopP = 1,
                FrequencyPenalty = 0.3,
                PresencePenalty = 0.3,
                StopSequences = new[] { "\n" }
            }, completion => completion.Text.Trim() != "" && completion.Text.Length <= 32,
                defaultResponse: ".");

            return result.Text.TrimEnd();
        }
    }
}