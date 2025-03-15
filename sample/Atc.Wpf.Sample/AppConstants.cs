namespace Atc.Wpf.Sample;

public static class AppConstants
{
    public const string CompanyName = "ATC";

    public const string AppIdentifier = "Atc.Wpf.Sample";

    public const string AppDisplayName = "Atc Wpf Sample";

    public static DirectoryInfo DataDirectory => new(
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            CompanyName,
            AppIdentifier));
}