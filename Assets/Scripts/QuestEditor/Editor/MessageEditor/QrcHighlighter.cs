using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using QuestEditor.Editor.MessageEditor.ColorScheme;
using UnityEditor;

namespace QuestEditor.Editor.MessageEditor
{
    public static class QrcHighlighter
    {
        private static Regex regex;
        private static MatchEvaluator evaluator;

        private static readonly Dictionary<string, string> ColorTable = new Dictionary<string, string>
        {
            { "timeClassName", Solarized.base01 },
            { "variable", Solarized.orange },
            { "centered", Solarized.green },
            { "faction", Solarized.magenta },
            { "placeName", Solarized.blue },
            { "location", Solarized.violet },
            { "symbol", Solarized.base1 },
        };

        private static string ToColoredCode(string code, string color)
        {
            return "<color=" + color + ">" + code + "</color>";
        }

        [InitializeOnLoadMethod]
        private static void Init()
        {
            const string pattern = "(?<{0}>({1}))";

            var patterns = new []
            {
                string.Format(pattern, "timeClassName", QrcSyntax.timeClassName),
                string.Format(pattern, "variable", QrcSyntax.variable),
                string.Format(pattern, "centered", QrcSyntax.centered),
                string.Format(pattern, "faction", QrcSyntax.faction),
                string.Format(pattern, "placeName", QrcSyntax.placeName),
                string.Format(pattern, "location", QrcSyntax.location),
                string.Format(pattern, "symbol", QrcSyntax.symbol)
            };
            string combinedPattern = "(" + string.Join("|", patterns) + ")";

            regex = new Regex(combinedPattern, RegexOptions.Compiled);

            evaluator = match =>
            {
                foreach (var pair in ColorTable.Where(pair => match.Groups[(string) pair.Key].Success))
                {
                    return ToColoredCode(match.Value, pair.Value);
                }

                return match.Value;
            };
        }

        public static string Highlight(string code)
        {
            return regex.Replace(code, evaluator);
        }
    }
}