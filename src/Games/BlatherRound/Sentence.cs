using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
// ReSharper disable InconsistentNaming

namespace JackboxGPT3.Games.BlatherRound
{
    public class Sentence
    {
        /// <summary>
        /// Phrases that should be used when the model thinks something is
        /// semantically similar to something else. At least one of these should
        /// be given for a response sentence.
        /// </summary>
        public static readonly string[] PHRASES_SIMILAR_TO = {
            "It's a lot like",
            "It's kinda similar to",
            "It has the same vibe as",
            "It has the same vibe as ", // ugh, trailing space lol
            "It's close to",
            "They're a lot like",
            "They're kinda similar to",
            "They have the same vibe as"
        };
        
        /// <summary>
        /// Phrases that should be used when the model thinks something is
        /// semantically dissimilar to something else. At least one of these should
        /// be given for a response sentence.
        /// </summary>
        public static readonly string[] PHRASES_DISSIMILAR_FROM = {
            "It's nothing like",
            "They're nothing like",
            "It's not the same form as"
        };

        [JsonProperty("words")]
        public IList<SentencePart> Parts { get; set; }
        
        [JsonProperty("type")]
        public SentenceType Type { get; set; }

        [JsonIgnore]
        public IList<string> FullSentence => Parts.Select(p => p.CurrentChoice).ToList();

        [JsonIgnore]
        public IList<string> ChosenWords => Parts.Where(p => p.HasChoice).Select(p => p.CurrentChoice).ToList();
    }

    public enum SentenceType
    {
        /// <summary>
        /// This is the first sentence the player writes for their given prompt.
        /// <inheritdoc cref="Call"/>
        /// </summary>
        [EnumMember(Value = "writing")]
        Writing,
        
        /// <summary>
        /// A sentence where the presenter incorporates the guesser's responses.
        /// 
        /// The first part will always be a "response-sentence" word, e.g. "It's a lot like",
        /// "It's kinda similar to", "It's nothing like", etc.
        ///
        /// The second part will always be the guessers' answers.
        /// </summary>
        [EnumMember(Value = "response")]
        Response,
        
        /// <summary>
        /// This sentence will have any number of options, with generally an adjective and
        /// a noun to choose.
        /// </summary>
        [EnumMember(Value = "call")]
        Call
    }
}