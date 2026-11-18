namespace Atc.Wpf.Components.Monitoring;

/// <summary>
/// Holds the filter settings (severities and text match) for the application monitor.
/// </summary>
public sealed partial class ApplicationFilterViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets a value indicating whether information-level entries are shown.
    /// </summary>
    [ObservableProperty]
    private bool severityInformation;

    /// <summary>
    /// Gets or sets a value indicating whether warning entries are shown.
    /// </summary>
    [ObservableProperty]
    private bool severityWarning;

    /// <summary>
    /// Gets or sets a value indicating whether error and critical entries are shown.
    /// </summary>
    [ObservableProperty]
    private bool severityError;

    /// <summary>
    /// Gets or sets the text that an entry must contain (case-insensitive) to be shown; empty shows all entries.
    /// </summary>
    [ObservableProperty]
    private string matchOnTextInData = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationFilterViewModel"/> class with all severities shown and no text filter.
    /// </summary>
    public ApplicationFilterViewModel()
    {
        SeverityInformation = true;
        SeverityWarning = true;
        SeverityError = true;
        MatchOnTextInData = string.Empty;
    }
}