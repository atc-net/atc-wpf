namespace Atc.Wpf.SampleControls;

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OK.")]
public class SampleViewerViewModel : ViewModelBase
{
    public SampleViewerViewModel()
    {
        Messenger.Default.Register<SampleItemMessage>(this, SampleItemMessageHandler);
    }

    private int tabSelectedIndex;
    private string? header;
    private UserControl? sampleContent;
    private string? xamlCode;
    private string? codeBehindCode;
    private string? viewModelCode;

    public int TabSelectedIndex
    {
        get => tabSelectedIndex;
        set
        {
            tabSelectedIndex = value;
            RaisePropertyChanged();
        }
    }

    public bool HasSampleContent => SampleContent is not null;

    public bool HasXamlCode => XamlCode is not null;

    public bool HasCodeBehindCode => CodeBehindCode is not null;

    public bool HasViewModelCode => ViewModelCode is not null;

    public string? Header
    {
        get => header;
        set
        {
            header = value;
            RaisePropertyChanged();
        }
    }

    public UserControl? SampleContent
    {
        get => sampleContent;
        set
        {
            sampleContent = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(HasSampleContent));
        }
    }

    public string? XamlCode
    {
        get => xamlCode;
        set
        {
            xamlCode = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(HasXamlCode));
        }
    }

    public string? CodeBehindCode
    {
        get => codeBehindCode;
        set
        {
            codeBehindCode = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(HasCodeBehindCode));
        }
    }

    public string? ViewModelCode
    {
        get => viewModelCode;
        set
        {
            viewModelCode = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(HasViewModelCode));
        }
    }

    private string ExtractClassName(
        string classFullName)
    {
        return classFullName.Split('.').Last();
    }

    private DirectoryInfo? ExtractBasePath(
        DirectoryInfo path)
    {
        if ("bin".Equals(path.Name, StringComparison.Ordinal))
        {
            return path.Parent;
        }

        return path.Parent is null
            ? null
            : ExtractBasePath(path.Parent);
    }

    private DirectoryInfo? ExtractSamplePath(
        FileSystemInfo baseLocation,
        string classViewName)
    {
        var files = Directory.GetFiles(baseLocation.FullName, $"{classViewName}.xaml", SearchOption.AllDirectories);
        return files.Length == 1
            ? new DirectoryInfo(files[0]).Parent
            : null;
    }

    private string? ReadFileText(
        string filePath)
    {
        return File.Exists(filePath)
            ? File.ReadAllText(filePath)
            : null;
    }

    private void SampleItemMessageHandler(
        SampleItemMessage obj)
    {
        TabSelectedIndex = 0;

        if (string.IsNullOrEmpty(obj.SampleItemPath))
        {
            ClearSelectedViewData();
        }
        else
        {
            if (obj.Header is not null)
            {
                SetSelectedViewData(obj.Header, obj.SampleItemPath);
            }
        }
    }

    private void ClearSelectedViewData()
    {
        Header = null;
        SampleContent = null;
        XamlCode = null;
        CodeBehindCode = null;
        ViewModelCode = null;
    }

    private void SetSelectedViewData(
        string sampleHeader,
        string samplePath)
    {
        var entryAssembly = Assembly.GetEntryAssembly();

        var sampleType = entryAssembly!
            .GetExportedTypes()
            .FirstOrDefault(x => x.FullName is not null && x.FullName.EndsWith(samplePath, StringComparison.Ordinal));

        if (sampleType is null)
        {
            _ = MessageBox.Show($"Can't find sample by path '{samplePath}'", "Error", MessageBoxButton.OK);
            return;
        }

        if (Activator.CreateInstance(sampleType) is not UserControl instance)
        {
            MessageBox.Show($"Can't create instance of sample by path '{samplePath}'", "Error", MessageBoxButton.OK);
            return;
        }

        var entryAssemblyLocation = new DirectoryInfo(Path.GetDirectoryName(entryAssembly.Location)!);
        var baseLocation = ExtractBasePath(entryAssemblyLocation);
        if (baseLocation is null)
        {
            MessageBox.Show("Can't find sample by invalid base location", "Error", MessageBoxButton.OK);
            return;
        }

        var classViewName = ExtractClassName(instance.ToString()!);
        var sampleLocation = ExtractSamplePath(baseLocation, classViewName);

        Header = sampleHeader;
        SampleContent = instance;
        XamlCode = ReadFileText(Path.Combine(sampleLocation!.FullName, classViewName + ".xaml"));
        CodeBehindCode = ReadFileText(Path.Combine(sampleLocation!.FullName, classViewName + ".xaml.cs"));

        if (instance.DataContext is null ||
            nameof(SampleViewerViewModel).Equals(ExtractClassName(instance.DataContext.ToString()!), StringComparison.Ordinal))
        {
            ViewModelCode = null;
        }
        else
        {
            var classViewModelName = ExtractClassName(instance.DataContext.ToString()!);
            ViewModelCode = ReadFileText(Path.Combine(sampleLocation!.FullName, classViewModelName + ".cs"));
        }
    }
}