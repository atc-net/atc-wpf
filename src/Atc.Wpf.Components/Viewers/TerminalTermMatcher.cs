namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// Whole-word keyword matching for the <see cref="TerminalViewer"/> line colouring.
/// </summary>
internal static class TerminalTermMatcher
{
    /// <summary>
    /// Returns <c>true</c> when <paramref name="term"/> occurs in <paramref name="line"/> as a whole word,
    /// that is, not directly preceded or followed by a letter, digit or underscore. Punctuation counts as a
    /// boundary, so <c>error</c> matches <c>error:</c>, <c>[error]</c> and <c>error.</c> but not <c>errors</c>.
    /// </summary>
    public static bool ContainsTermAsWord(
        ReadOnlySpan<char> line,
        ReadOnlySpan<char> term,
        StringComparison comparison)
    {
        if (term.IsEmpty)
        {
            return false;
        }

        // Search from an offset into the full line, so the character before a later occurrence is still visible
        // for the boundary check (slicing the line away would make "aa" match inside "xaaa").
        var start = 0;
        while (start <= line.Length - term.Length)
        {
            var found = line[start..].IndexOf(term, comparison);
            if (found < 0)
            {
                return false;
            }

            var index = start + found;
            var end = index + term.Length;
            var atStart = index == 0 || !IsWordChar(line[index - 1]);
            var atEnd = end == line.Length || !IsWordChar(line[end]);

            if (atStart && atEnd)
            {
                return true;
            }

            start = index + 1;
        }

        return false;
    }

    private static bool IsWordChar(char c)
        => char.IsLetterOrDigit(c) || c == '_';
}