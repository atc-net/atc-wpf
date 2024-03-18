namespace Atc.Wpf.Controls.Options;

public class BasicApplicationOptions
{
    public const string SectionName = "Application";

    public string Theme { get; set; } = "Light.Blue";

    public string Language { get; set; } = "en-US";

    public bool OpenRecentFileOnStartup { get; set; }

    public override string ToString()
        => $"{nameof(Theme)}: {Theme}, {nameof(Language)}: {Language}, {nameof(OpenRecentFileOnStartup)}: {OpenRecentFileOnStartup}";
}