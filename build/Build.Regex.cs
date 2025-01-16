using System.Text.RegularExpressions;

sealed partial class Build
{
    readonly Regex StreamRegex = StreamRegexGenerator();
    readonly Regex YearRegex = YearRegexGenerator();

    [GeneratedRegex("'(.+?)'", RegexOptions.Compiled)]
    private static partial Regex StreamRegexGenerator();

    [GeneratedRegex(@"\d{4}")]
    private static partial Regex YearRegexGenerator();
}