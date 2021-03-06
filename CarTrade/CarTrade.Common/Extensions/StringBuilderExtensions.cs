using System.Text;

namespace CarTrade.Common.Extensions
{
    public static class StringBuilderExtensions
    {
        private const string MessageNewLine = "<br />";

        public static void AppendEmailNewLine(this StringBuilder sb, string value)
        {
            sb.Append(value + MessageNewLine);
        }
    }
}
