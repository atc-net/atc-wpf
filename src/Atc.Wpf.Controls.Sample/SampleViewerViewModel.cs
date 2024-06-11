// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Controls.Sample;

public class SampleViewerViewModel : ViewModelBase
{
    public SampleViewerViewModel()
    {
        Messenger.Default.Register<SampleItemMessage>(this, SampleItemMessageHandler);
    }

    private FileInfo[]? readmeMarkdownFiles;
    private int tabSelectedIndex;
    private string? header;
    private UserControl? sampleContent;
    private string? xamlCode;
    private string? codeBehindCode;
    private string? viewModelCode;
    private string? readmeMarkdown;

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

    public bool HasReadmeMarkdown => ReadmeMarkdown is not null;

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

    public string? ReadmeMarkdown
    {
        get => readmeMarkdown;
        set
        {
            readmeMarkdown = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(HasReadmeMarkdown));
        }
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
        var entryAssembly = Assembly.GetEntryAssembly()!;
        var sampleType = GetTypeBySamplePath(entryAssembly, samplePath);

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

        var sampleTypeAssemblyLocation = new DirectoryInfo(Path.GetDirectoryName(sampleType.Assembly.Location)!);
        var baseLocation = ExtractBasePath(sampleTypeAssemblyLocation);
        if (baseLocation is null)
        {
            MessageBox.Show("Can't find sample by invalid base location", "Error", MessageBoxButton.OK);
            return;
        }

        var classViewName = ExtractClassName(instance.ToString()!);
        var sampleLocation = ExtractSamplePath(baseLocation, classViewName, sampleType);
        if (sampleLocation is null)
        {
            MessageBox.Show("Can't find sample by invalid location", "Error", MessageBoxButton.OK);
            return;
        }

        Header = sampleHeader;
        SampleContent = instance;
        XamlCode = ReadFileText(Path.Combine(sampleLocation.FullName, classViewName + ".xaml"));
        CodeBehindCode = ReadFileText(Path.Combine(sampleLocation.FullName, classViewName + ".xaml.cs"));

        if (instance.DataContext is null ||
            nameof(SampleViewerViewModel).Equals(ExtractClassName(instance.DataContext.ToString()!), StringComparison.Ordinal))
        {
            ViewModelCode = null;
        }
        else
        {
            var classViewModelName = ExtractClassName(instance.DataContext.ToString()!);
            ViewModelCode = ReadFileText(Path.Combine(sampleLocation.FullName, classViewModelName + ".cs"));
        }

        LoadAndRenderReadmeMarkdownIfPossible(classViewName);
    }

    private void LoadAndRenderReadmeMarkdownIfPossible(
        string classViewName)
    {
        ReadmeMarkdown = null;
        if (readmeMarkdownFiles is null)
        {
            PrepareReadmeReferences();
        }

        if (readmeMarkdownFiles is null)
        {
            return;
        }

        var readmeMarkdownFile = readmeMarkdownFiles.SingleOrDefault(x => x.Name.StartsWith(classViewName + "_Readme", StringComparison.OrdinalIgnoreCase));
        if (readmeMarkdownFile is null &&
            classViewName.EndsWith("View", StringComparison.Ordinal))
        {
            var className = classViewName.Replace("View", string.Empty, StringComparison.Ordinal);
            readmeMarkdownFile = readmeMarkdownFiles.SingleOrDefault(x => x.Name.StartsWith(className + "_Readme", StringComparison.OrdinalIgnoreCase));

            if (readmeMarkdownFile is null)
            {
                var type = FindCustomTypeByName(className);
                if (type?.FullName is not null)
                {
                    var sa = type.FullName.Split('.', StringSplitOptions.RemoveEmptyEntries);
                    if (sa.Length > 2)
                    {
                        var classFolder = sa[^2];
                        readmeMarkdownFile = readmeMarkdownFiles.SingleOrDefault(x => x.FullName.EndsWith($"\\{classFolder}\\@Readme.md", StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
        }

        if (readmeMarkdownFile is null)
        {
            return;
        }

        var readmeMarkdownTxt = FileHelper.ReadAllText(readmeMarkdownFile);
        ReadmeMarkdown = readmeMarkdownTxt;
    }

    private static Type? FindCustomTypeByName(string className)
    {
        var type = Type.GetType(className);
        if (type is not null)
        {
            return type;
        }

        var customAssemblies = AppDomain
            .CurrentDomain
            .GetCustomAssemblies()
            .OrderBy(x => x.FullName, StringComparer.Ordinal);

        foreach (var customAssembly in customAssemblies)
        {
            var exportedTypes = customAssembly.GetExportedTypes();

            type = exportedTypes.FirstOrDefault(x => x.Name.Equals(className, StringComparison.Ordinal));

            if (type is not null)
            {
                break;
            }
        }

        return type;
    }

    private static string GetBasePath()
    {
        var entryAssembly = Assembly.GetEntryAssembly()!;
        var assemblyLocation = entryAssembly.Location;

        var indexOf = assemblyLocation.IndexOf("\\sample", StringComparison.OrdinalIgnoreCase);
        var baseLocation = indexOf == -1
            ? assemblyLocation
            : assemblyLocation[..indexOf];

        return baseLocation;
    }

    public void PrepareReadmeReferences()
    {
        var basePath = GetBasePath();

        readmeMarkdownFiles = FileHelper.GetFiles(basePath, "*.md")
            .Where(x => x.Name.Contains("readme", StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    private static Type? GetTypeBySamplePath(
        Assembly entryAssembly,
        string samplePath)
    {
        var sampleType = entryAssembly
            .GetExportedTypes()
            .FirstOrDefault(x => x.FullName is not null && x.FullName.EndsWith(samplePath, StringComparison.Ordinal));

        if (sampleType is not null)
        {
            return sampleType;
        }

        var assemblyStartName = entryAssembly.GetName().Name!.Split('.')[0];
        foreach (var assembly in AppDomain.CurrentDomain
                     .GetAssemblies()
                     .Where(x => !x.IsDynamic &&
                                 x.FullName!.StartsWith(assemblyStartName, StringComparison.Ordinal)))
        {
            sampleType = assembly
                .GetExportedTypes()
                .FirstOrDefault(x => x.FullName is not null && x.FullName.EndsWith(samplePath, StringComparison.Ordinal));

            if (sampleType is not null)
            {
                return sampleType;
            }
        }

        return null;
    }

    [SuppressMessage("Performance", "MA0098:Use indexer instead of LINQ methods", Justification = "OK.")]
    private static string ExtractClassName(
        string classFullName)
    {
        return classFullName.Split('.').Last();
    }

    private static DirectoryInfo? ExtractBasePath(
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

    private static DirectoryInfo? ExtractSamplePath(
        FileSystemInfo baseLocation,
        string classViewName,
        Type sampleType)
    {
        var files = Directory.GetFiles(baseLocation.FullName, $"{classViewName}.xaml", SearchOption.AllDirectories);
        switch (files.Length)
        {
            case 0:
                return null;
            case 1:
                return new DirectoryInfo(files[0]).Parent;
        }

        foreach (var file in files)
        {
            var s = file.Replace('\\', '.');
            if (s.Contains(sampleType.FullName!, StringComparison.OrdinalIgnoreCase))
            {
                return new DirectoryInfo(file).Parent;
            }
        }

        return null;
    }

    private static string? ReadFileText(
        string filePath)
    {
        return File.Exists(filePath)
            ? File.ReadAllText(filePath)
            : null;
    }
}