namespace Atc.Wpf.Components.Monitoring;

/// <summary>
/// Holds the filter settings (severities and text match) for the application monitor.
/// </summary>
public sealed class ApplicationFilterViewModel : ViewModelBase
{
    private bool severityInformation;
    private bool severityWarning;
    private bool severityError;
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

    /// <summary>
    /// Gets or sets a value indicating whether information-level entries are shown.
    /// </summary>
    public bool SeverityInformation
    {
        get => severityInformation;
        set
        {
            if (value == severityInformation)
            {
                return;
            }

            severityInformation = value;
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether warning entries are shown.
    /// </summary>
    public bool SeverityWarning
    {
        get => severityWarning;
        set
        {
            if (value == severityWarning)
            {
                return;
            }

            severityWarning = value;
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether error and critical entries are shown.
    /// </summary>
    public bool SeverityError
    {
        get => severityError;
        set
        {
            if (value == severityError)
            {
                return;
            }

            severityError = value;
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the text that an entry must contain (case-insensitive) to be shown; empty shows all entries.
    /// </summary>
    public string MatchOnTextInData
    {
        get => matchOnTextInData;
        set
        {
            if (value == matchOnTextInData)
            {
                return;
            }

            matchOnTextInData = value;
            RaisePropertyChanged();
        }
    }
}