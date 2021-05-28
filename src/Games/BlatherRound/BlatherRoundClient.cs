using System;
using System.Collections.Generic;
using System.Linq;
using JackboxGPT3.Games.BlatherRound.Models;
using JackboxGPT3.Games.Common;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Services;
using Newtonsoft.Json;
using Serilog;

namespace JackboxGPT3.Games.BlatherRound
{
    public class BlatherRoundClient : PlayerSerializedClient<BlatherRoundRoom, BlatherRoundPlayer>
    {
        private const string KEY_TEXT_DESCRIPTIONS = "textDescriptions";
        
        private const string TD_KEY_NEW_SENTENCE = "TD_CATEGORY_NEW_SENTENCE";
        private const string TD_KEY_PLAYER_IS_PRESENTING = "TD_CATEGORY_PLAYER_IS_PRESENTING";

        public event EventHandler<string> OnNewSentence;
        public event EventHandler<Sentence> OnWriteNewSentence;
        public event EventHandler OnPlayerStartedPresenting;
        
        public string CurrentCategory { get; private set; }
        public List<string> CurrentSentences { get; private set; }
        public Sentence CurrentSentence { get; private set; }

        public BlatherRoundClient(IConfigurationProvider configuration, ILogger logger) : base(configuration, logger)
        {
            CurrentSentences = new List<string>();
            OnSelfUpdate += PreSelfUpdate;
        }
        
        private void SubmitEntry<T>(T entry)
        {
            var req = new SubmitEntryRequest<T>(entry);
            ClientSend(req);
        }
        
        public void SubmitGuess(string guess)
        {
            var req = new TextUpdateRequest(_gameState.Self.TextKey, guess);
            WsSend(TextUpdateRequest.OpCode, req);
        }

        public void ChoosePassword(int index)
        {
            var req = new ChooseRequest<int>(index);
            ClientSend(req);
        }

        public void ChooseWord(int position, int index)
        {
            if (CurrentSentence == null) return;
            CurrentSentence.Parts[position].CurrentChoiceIndex = index;
            CurrentSentence.Parts[position].HasChoice = true;

            var entry = new ChangedWordEntry
            {
                ChangedWord = CurrentSentence.Parts[position].CurrentChoice,
                Position = new ChangedWordEntry.WordPosition
                {
                    Sentence = position
                },
                Sentence = CurrentSentence.FullSentence
            };
            
            SubmitEntry(entry);
        }

        public void SubmitSentence(bool skip = false)
        {
            if (CurrentSentence == null) return;

            var sentence = skip ? new List<string>() : CurrentSentence.FullSentence;
            var entry = new SubmitSentenceEntry(sentence);
            SubmitEntry(entry);

            CurrentSentence = null;
        }

        private void PreSelfUpdate(object sender, Revision<BlatherRoundPlayer> revision)
        {
            if (revision.Old.State == revision.New.State || revision.New.State != PlayerState.MakeSentence) return;
            
            CurrentSentence = revision.New.Sentence;
            OnWriteNewSentence?.Invoke(this, CurrentSentence);
        }

        protected override void HandleOperation(IOperation op)
        {
            base.HandleOperation(op);

            // praise Jackbox for including screen reader support 😌
            if (op.Key != KEY_TEXT_DESCRIPTIONS) return;
            
            var descriptions = JsonConvert.DeserializeObject<TextDescriptions>(op.Value);

            foreach (var description in descriptions.LatestDescriptions)
            {
                switch (description.Category)
                {
                    case TD_KEY_NEW_SENTENCE:
                        CurrentSentences.Add(description.Text.Trim());
                        OnNewSentence?.Invoke(this, description.Text.Trim());
                        break;
                    case TD_KEY_PLAYER_IS_PRESENTING:
                        CurrentSentences.Clear();
                        CurrentCategory = description.Text.Split(":").Last().Trim().ToLower();
                        OnPlayerStartedPresenting?.Invoke(this, new EventArgs());
                        break;
                }
            }
        }
    }
}