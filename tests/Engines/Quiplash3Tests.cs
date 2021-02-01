using FluentAssertions;
using JackboxGPT3.Engines;
using NUnit.Framework;

namespace Tests.Engines
{
    public class Quiplash3Tests
    {
        [Test]
        public void ShouldCleanPromptForEntry()
        {
            const string rawPrompt =
                "<div class='header'>Prompt 1 of 2</div><div>If you’re being honest with yourself, you’ll never be brave enough to _______ <i>some italics</i>  </div>";
            const string expectedPrompt =
                "if you’re being honest with yourself, you’ll never be brave enough to _______ some italics";

            Quiplash3Engine.CleanPromptForEntry(rawPrompt).Should().Be(expectedPrompt);
        }

        [Test]
        public void ShouldCleanPromptForSelection()
        {
            const string rawPrompt =
                "    Your last thought before passing into the afterlife   <br /><br />Vote for your favorite";
            const string expectedPrompt =
                "your last thought before passing into the afterlife";

            Quiplash3Engine.CleanPromptForSelection(rawPrompt).Should().Be(expectedPrompt);
        }

        [Test]
        public void ShouldCleanQuipWithSpecialCharacters()
        {
            const string rawQuip =
                "\u0026QUOT;THE GAME OF LIFE\u0026QUOT;";
            const string expectedQuip =
                "\"the game of life\"";

            Quiplash3Engine.CleanQuipForSelection(rawQuip).Should().Be(expectedQuip);
        }
        
        [Test]
        public void ShouldCleanThripForSelection()
        {
            const string rawThrip =
                "<div>  One</div><div>Two</div><div>And finally, three  </div>";
            const string expectedThrip =
                "one|two|and finally, three";

            Quiplash3Engine.CleanThripForSelection(rawThrip).Should().Be(expectedThrip);
        }
    }
}