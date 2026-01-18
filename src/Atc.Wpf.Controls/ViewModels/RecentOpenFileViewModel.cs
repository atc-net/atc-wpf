namespace Atc.Wpf.Controls.ViewModels;

public sealed partial class RecentOpenFileViewModel : ViewModelBase
{
    private readonly DirectoryInfo applicationDataDirectory;
    [ObservableProperty] private DateTime timeStamp;
    [ObservableProperty(DependentPropertyNames = [nameof(FileDisplay)])] private string file = string.Empty;

    [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "OK.")]
    public RecentOpenFileViewModel()
        => applicationDataDirectory = new DirectoryInfo(@"C:\");

    public RecentOpenFileViewModel(
        DirectoryInfo applicationDataDirectory,
        DateTime timeStamp,
        string file)
    {
        ArgumentNullException.ThrowIfNull(applicationDataDirectory);
        ArgumentNullException.ThrowIfNull(file);

        this.applicationDataDirectory = applicationDataDirectory;
        TimeStamp = timeStamp;
        File = file;
    }

    public string FileDisplay
    {
        get
        {
            if (!file.StartsWith(applicationDataDirectory.FullName, StringComparison.Ordinal))
            {
                return file;
            }

            var fileInfo = new FileInfo(file);
            return $"{fileInfo.Directory!.Name} - {fileInfo.Name}";
        }
    }

    public override string ToString()
        => $"{nameof(TimeStamp)}: {TimeStamp}, {nameof(File)}: {File}, {nameof(FileDisplay)}: {FileDisplay}";
}