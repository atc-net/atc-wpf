namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// Matches terminal lines against the <see cref="TerminalViewer"/> search: a case-insensitive substring or, when
/// <see cref="UseRegex"/> is set, a case-insensitive regular expression with a per-line time limit.
/// </summary>
/// <remarks>
/// A regular expression that runs past <see cref="MatchTimeout"/> on a line counts as no match. The pattern is then
/// flagged (<see cref="HasTimedOut"/>) and no longer evaluated until it changes, so a catastrophic pattern cannot stall
/// the UI thread for <see cref="MatchTimeout"/> on every line, and the timeout never escapes into a filter or batch.
/// </remarks>
internal sealed class TerminalSearchMatcher
{
    /// <summary>
    /// The time a regular expression may spend on one line.
    /// </summary>
    public static readonly TimeSpan MatchTimeout = TimeSpan.FromMilliseconds(50);

    private Regex? regex;

    /// <summary>
    /// Gets the search text. Empty when no search is active.
    /// </summary>
    public string Pattern { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether <see cref="Pattern"/> is a regular expression.
    /// </summary>
    public bool UseRegex { get; private set; }

    /// <summary>
    /// Gets a value indicating whether a search is active (the pattern is not empty).
    /// </summary>
    public bool IsActive
        => Pattern.Length > 0;

    /// <summary>
    /// Gets a value indicating whether the regular expression exceeded <see cref="MatchTimeout"/>; it then matches
    /// nothing until the pattern changes.
    /// </summary>
    public bool HasTimedOut { get; private set; }

    /// <summary>
    /// Sets the search pattern.
    /// </summary>
    /// <returns><see langword="true"/> when the pattern or mode changed.</returns>
    public bool Update(
        string? pattern,
        bool useRegex)
    {
        pattern ??= string.Empty;
        if (string.Equals(pattern, Pattern, StringComparison.Ordinal) && useRegex == UseRegex)
        {
            return false;
        }

        Pattern = pattern;
        UseRegex = useRegex;
        HasTimedOut = false;
        regex = null;

        if (useRegex && pattern.Length > 0)
        {
            try
            {
                regex = new Regex(
                    pattern,
                    RegexOptions.IgnoreCase | RegexOptions.Compiled,
                    MatchTimeout);
            }
            catch (ArgumentException)
            {
                // Invalid pattern: matches nothing.
            }
        }

        return true;
    }

    /// <summary>
    /// Returns whether <paramref name="text"/> matches the search. Every line matches when no search is active.
    /// </summary>
    public bool IsMatch(string text)
    {
        if (!IsActive)
        {
            return true;
        }

        if (!UseRegex)
        {
            return text.Contains(Pattern, StringComparison.OrdinalIgnoreCase);
        }

        if (regex is null || HasTimedOut)
        {
            return false;
        }

        try
        {
            return regex.IsMatch(text);
        }
        catch (RegexMatchTimeoutException)
        {
            HasTimedOut = true;
            return false;
        }
    }
}