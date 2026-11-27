namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// The decisions <see cref="SampleApplication"/> makes at start-up, kept apart so they are tested without an
/// application.
/// </summary>
internal static class SampleApplicationRules
{
    private const string PackApplicationPrefix = "pack://application:,,,";

    /// <summary>
    /// Gets the Atc style dictionaries a sample app needs, in the order they are merged.
    /// </summary>
    public static IReadOnlyList<Uri> StyleDictionaries { get; } =
    [
        new($"{PackApplicationPrefix}/Atc.Wpf.Theming;component/Styles/Default.xaml", UriKind.Absolute),
        new($"{PackApplicationPrefix}/Atc.Wpf.Controls;component/Styles/Controls.xaml", UriKind.Absolute),
        new($"{PackApplicationPrefix}/Atc.Wpf.Components;component/Styles/Controls.xaml", UriKind.Absolute),
    ];

    /// <summary>
    /// Determines whether data binding errors are traced: always with a debugger, else when the environment
    /// variable is <c>1</c>.
    /// </summary>
    /// <param name="isDebuggerAttached">Whether a debugger is attached.</param>
    /// <param name="variableName">The name of the environment variable, or <see langword="null"/> for none.</param>
    /// <param name="getEnvironmentVariable">Reads an environment variable.</param>
    /// <returns><see langword="true"/> to trace binding errors.</returns>
    public static bool ShouldTraceBindingErrors(
        bool isDebuggerAttached,
        string? variableName,
        Func<string, string?> getEnvironmentVariable)
    {
        if (isDebuggerAttached)
        {
            return true;
        }

        return !string.IsNullOrEmpty(variableName) &&
               string.Equals(getEnvironmentVariable(variableName), "1", StringComparison.Ordinal);
    }

    /// <summary>
    /// Gets the <see cref="StyleDictionaries"/> not among the merged dictionary sources (compared without the
    /// <c>pack://application:,,,</c> prefix, ignoring case).
    /// </summary>
    /// <param name="mergedSources">The sources of the dictionaries already merged.</param>
    /// <returns>The missing style dictionaries, in merge order.</returns>
    public static IReadOnlyList<Uri> MissingStyleDictionaries(
        IEnumerable<Uri?> mergedSources)
    {
        var merged = mergedSources
            .Where(x => x is not null)
            .Select(x => Normalize(x!))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return StyleDictionaries
            .Where(x => !merged.Contains(Normalize(x)))
            .ToList();
    }

    /// <summary>
    /// Gets the sources of the dictionaries merged into a resource dictionary, nested ones included.
    /// </summary>
    /// <param name="resources">The resource dictionary.</param>
    /// <returns>The sources; <see langword="null"/> for a dictionary without one.</returns>
    public static IEnumerable<Uri?> MergedSources(ResourceDictionary resources)
    {
        foreach (var dictionary in resources.MergedDictionaries)
        {
            yield return dictionary.Source;
            foreach (var source in MergedSources(dictionary))
            {
                yield return source;
            }
        }
    }

    /// <summary>
    /// Lets one unhandled-exception dialog be shown at a time: an exception raised again while its dialog's
    /// message loop runs (e.g. in a page's layout or a timer) does not stack another dialog on top.
    /// </summary>
    internal sealed class ExceptionDialogGate
    {
        private bool isShowing;

        /// <summary>
        /// Tries to start showing a dialog.
        /// </summary>
        /// <returns><see langword="true"/> when no dialog is shown; <see langword="false"/> while one is.</returns>
        public bool TryEnter()
        {
            if (isShowing)
            {
                return false;
            }

            isShowing = true;
            return true;
        }

        /// <summary>
        /// Ends showing the dialog.
        /// </summary>
        public void Exit()
            => isShowing = false;
    }

    private static string Normalize(Uri source)
    {
        var text = source.OriginalString;
        if (text.StartsWith(PackApplicationPrefix, StringComparison.OrdinalIgnoreCase))
        {
            text = text[PackApplicationPrefix.Length..];
        }

        return text.TrimStart('/');
    }
}