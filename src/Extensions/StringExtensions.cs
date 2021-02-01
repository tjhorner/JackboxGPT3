using System.Text.RegularExpressions;

namespace JackboxGPT3.Extensions
{
    public static class StringExtensions
    {
        public static string StripHtml(this string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}
