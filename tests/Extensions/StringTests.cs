using FluentAssertions;
using JackboxGPT3.Extensions;
using NUnit.Framework;

namespace tests.Extensions
{
    public class StringTests
    {
        [Test]
        public void ShouldStripHtmlTags()
        {
            var pre = "<b>hello</b> <i>I have <b>nested!</b> HTML tags</i> <body></body>";
            var post = pre.StripHtml();

            post.Should().Be("hello I have nested! HTML tags ");
        }
    }
}
