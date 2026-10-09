namespace Atc.Wpf.Controls.ViewModels;

/// <summary>View model for an entry in the list of recently opened files.</summary>
public sealed partial class RecentOpenFileViewModel : ViewModelBase
{
    private readonly DirectoryInfo applicationDataDirectory;
    [ObservableProperty] private DateTime timeStamp;
    [ObservableProperty(DependentPropertyNames = [nameof(FileDisplay)])] private string file = string.Empty;

    /// <summary>Initializes a new instance of the <see cref="RecentOpenFileViewModel"/> class.</summary>
    [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "OK.")]
    public RecentOpenFileViewModel()
        => applicationDataDirectory = new DirectoryInfo(@"C:\");

    /// <summary>Initializes a new instance of the <see cref="RecentOpenFileViewModel"/> class for the specified file.</summary>
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

    /// <summary>Gets the file path for display; files under the application data directory are shown as "folder - file name".</summary>
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

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(TimeStamp)}: {TimeStamp}, {nameof(File)}: {File}, {nameof(FileDisplay)}: {FileDisplay}";
}