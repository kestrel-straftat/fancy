using System.Text.RegularExpressions;

namespace Fancy;

internal static class Extensions
{
    // adapted from https://stackoverflow.com/questions/5796383/insert-spaces-between-words-on-a-camel-cased-token
    private static readonly Regex splitCamelCaseA = new(@"(\p{Lu})(\p{Lu})(\p{Ll})", RegexOptions.Compiled);
    private static readonly Regex splitCamelCaseB = new(@"(\p{Ll})(\p{Lu})", RegexOptions.Compiled);
    
    public static string SplitCamelCase(this string str) {
        return splitCamelCaseB.Replace(
            splitCamelCaseA.Replace(str, "$1 $2"),
            "$1 $2"
        );
    }
}