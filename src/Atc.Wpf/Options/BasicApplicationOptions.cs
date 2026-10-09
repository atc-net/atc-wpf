namespace Atc.Wpf.Options;

/// <summary>
/// Basic application options bound from the application settings.
/// </summary>
public class BasicApplicationOptions
{
    /// <summary>The configuration section name.</summary>
    public const string SectionName = "Application";

    /// <summary>
    /// Gets or sets the theme name (for example <c>Light.Blue</c>).
    /// </summary>
    public string Theme { get; set; } = "Light.Blue";

    /// <summary>
    /// Gets or sets the UI language as a culture name (for example <c>en-US</c>).
    /// </summary>
    public string Language { get; set; } = "en-US";

    /// <summary>
    /// Gets or sets a value indicating whether the most recently opened file is opened on startup.
    /// </summary>
    public bool OpenRecentFileOnStartup { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Theme)}: {Theme}, {nameof(Language)}: {Language}, {nameof(OpenRecentFileOnStartup)}: {OpenRecentFileOnStartup}";
}