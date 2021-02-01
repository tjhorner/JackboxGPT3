using FluentAssertions;
using JackboxGPT3.Extensions;
using NUnit.Framework;

namespace Tests.Extensions
{
    public class StringTests
    {
        [Test]
        public void ShouldStripHtmlTags()
        {
            const string pre = "<b>hello</b> <i>I have <b>nested!</b> HTML tags</i> <body></body>";
            var post = pre.StripHtml();

            post.Should().Be("hello I have nested! HTML tags ");
        }
    }
}
