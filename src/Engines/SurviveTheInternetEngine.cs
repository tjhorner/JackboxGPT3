using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JackboxGPT3.Extensions;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Games.SurviveTheInternet;
using JackboxGPT3.Games.SurviveTheInternet.Models;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Engines
{
    public class SurviveTheInternetEngine : BaseJackboxEngine<SurviveTheInternetClient>
    {
        protected override string Tag => "survivetheinternet";

        private readonly ImageDescriptionProvider _descriptionProvider;

        public SurviveTheInternetEngine(ICompletionService completionService, ILogger logger,
            SurviveTheInternetClient client) : base(completionService, logger, client)
        {
            _descriptionProvider = new ImageDescriptionProvider("sti_image_descriptions.json");
            
            JackboxClient.OnSelfUpdate += OnSelfUpdate;
            JackboxClient.Connect();
        }

        private void OnSelfUpdate(object sender, Revision<SurviveTheInternetPlayer> revision)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (revision.New.State)
            {
                case RoomState.EnterSingleText when !revision.New.Entry:
                    SubmitEntry(revision.New);
                    break;
                case RoomState.Voting when revision.New.Chosen == null:
                    SubmitVote(revision.New);
                    break;
                case RoomState.MakeSingleChoice when !revision.New.Entry:
                    SubmitImageChoice(revision.New);
                    break;
            }
        }

        private void SubmitImageChoice(SurviveTheInternetPlayer player)
        {
            var choice = player.ImageChoices.RandomIndex();
            JackboxClient.ChooseIndex(choice);
        }

        private void SubmitVote(SurviveTheInternetPlayer player)
        {
            var vote = player.EntryChoices.RandomIndex();
            JackboxClient.ChooseIndex(vote);
        }

        private async void SubmitEntry(SurviveTheInternetPlayer player)
        {
            string entry;

            if (player.Text.AboveBlackBox != null && player.Text.AboveBlackBox.StartsWith("<img"))
                entry = await ProvideImageTwist(player.Text, player.MaxLength);
            else
                entry = player.EntryId switch
                {
                    "response" => await ProvideResponse(player.Text.BelowBlackBox, player.MaxLength),
                    "twist" => await ProvideTwist(player.Text, player.MaxLength),
                    _ => "I don't know how to respond to that"
                };

            LogDebug($"Submitting entry \"{entry}\".");

            JackboxClient.SendEntry(entry);
        }
        
        #region GPT-3 Prompts
        private async Task<string> ProvideResponse(string stiPrompt, int maxLength)
        {
            var prompt = $@"In the first part of the game Survive the Internet, players are asked questions which they should answer short and concisely. For example:

Q: How's your retirement fund doing?
A: It's nonexistant!

Q: What are your thoughts on professional wrestling?
A: It's all so fake.

Q: Describe an attitude you admire.
A: I love positive people!

Q: {stiPrompt}
A:";
            
            LogDebug($"GPT-3 Prompt: {prompt}");
            
            var result = await CompletionService.CompletePrompt(prompt, new ICompletionService.CompletionParameters
                {
                    Temperature = 0.7,
                    MaxTokens = 32,
                    TopP = 1,
                    FrequencyPenalty = 0.3,
                    PresencePenalty = 0.2,
                    StopSequences = new[] { "\n" }
                }, completion => completion.Text.Length <= maxLength,
                defaultResponse: "I dunno");

            return result.Text.Trim().TrimQuotes().TrimEnd('.');
        }
        
        private async Task<string> ProvideTwist(TextPrompt stiPrompt, int maxLength)
        {
            var prompt = $@"Below are some responses from the party game Survive the Internet. The goal of this game is to take another player's words and twist them to make the other player look ridiculous.

""Some things are best kept in the dark"" would be a silly comment to a crowdfunding campaign titled: Help Me Fund My Movie
""I'm skeptical"" would be a ridiculous response to this comment: She said yes!
""Too much nudity"" would be a ridiculous comment to a video titled: How to Play Guitar
""Yawn"" would be a terrible comment in response to this news headline: Bank Robber on the Loose
""The bathroom"" would be a ridiculous answer to this question: Where do you cry the most?
""Let's hunt him down"" would be a terrible comment in response to this news headline: Local Man Wins Lottery
""Not that impressive tbh"" would be a ridiculous comment to a video titled: Johnny Learns How to Ride a Bike!
""It's not the most comfortable thing to sit on"" would be a ridiculous review for a product called: 18-inch Wooden Spoon
""{stiPrompt.BlackBox}"" {stiPrompt.BelowBlackBox.ToLower().Trim()}";
            
            LogDebug($"GPT-3 Prompt: {prompt}");
            
            var result = await CompletionService.CompletePrompt(prompt, new ICompletionService.CompletionParameters
                {
                    Temperature = 0.7,
                    MaxTokens = 32,
                    TopP = 1,
                    FrequencyPenalty = 0.3,
                    PresencePenalty = 0.2,
                    StopSequences = new[] { "\n" }
                }, completion => completion.Text.Length <= maxLength,
                defaultResponse: "I dunno");

            return result.Text.Trim().TrimQuotes().TrimEnd('.');
        }
        
        private async Task<string> ProvideImageTwist(TextPrompt stiPrompt, int maxLength)
        {
            var description = _descriptionProvider.ProvideDescriptionForImageId(GetImageId(stiPrompt));
            
            var prompt = $@"Below are some responses from the party game Survive the Internet. In the final round, each player takes an image and tries to come up with a caption that would make the other players look crazy or ridiculous.

An absurd and ridiculous Instagram caption for a photo of a group of mailboxes, with one open: Learned how to lock pick earlier. Score!
An absurd and ridiculous Instagram caption for a photo of people's legs through bathroom stalls: Just asked these guys how they were doing. They didn't respond.
An absurd and ridiculous Instagram caption for a photo of a group of people posing for a photo at a funeral: Funeral? I thought this was a party.
An absurd and ridiculous Instagram caption for a photo of {description}:";
            
            LogDebug($"GPT-3 Prompt: {prompt}");
            
            var result = await CompletionService.CompletePrompt(prompt, new ICompletionService.CompletionParameters
                {
                    Temperature = 0.7,
                    MaxTokens = 32,
                    TopP = 1,
                    FrequencyPenalty = 0.3,
                    PresencePenalty = 0.2,
                    StopSequences = new[] { "\n" }
                }, completion => completion.Text.Length <= maxLength,
                defaultResponse: "I dunno");

            return result.Text.Trim().TrimQuotes().TrimEnd('.');
        }

        private static string GetImageId(TextPrompt stiPrompt)
        {
            const string pattern = @"[A-z]+\.jpg";
            var match = Regex.Match(stiPrompt.AboveBlackBox, pattern);
            return match.Success ? match.Value : "Baseball.jpg"; // why not
        }
        #endregion
    }
}