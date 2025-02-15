// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Controls.Sample;

public sealed class SampleViewerViewModel : ViewModelBase
{
    public SampleViewerViewModel()
    {
        Messenger.Default.Register<SampleItemMessage>(this, SampleItemMessageHandler);
    }

    private FileInfo[]? markdownDocumentsFiles;
    private int tabSelectedIndex;
    private string? header;
    private UserControl? sampleContent;
    private string? xamlCode;
    private string? codeBehindCode;
    private string? viewModelCode;
    private string? markdownDocument;
    private bool startOnMarkdownDocument;

    public int TabSelectedIndex
    {
        get => tabSelectedIndex;
        set
        {
            if (tabSelectedIndex == value)
            {
                return;
            }

            tabSelectedIndex = value;
            RaisePropertyChanged();
        }
    }

    public bool HasSampleContent => SampleContent is not null;

    public bool HasXamlCode => XamlCode is not null;

    public bool HasCodeBehindCode => CodeBehindCode is not null;

    public bool HasViewModelCode => ViewModelCode is not null;

    public bool HasMarkdownDocument => MarkdownDocument is not null;

    public string? Header
    {
        get => header;
        set
        {
            if (header == value)
            {
                return;
            }

            header = value;
            RaisePropertyChanged();
        }
    }

    public UserControl? SampleContent
    {
        get => sampleContent;
        set
        {
            if (sampleContent == value)
            {
                return;
            }

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
            if (xamlCode == value)
            {
                return;
            }

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
            if (codeBehindCode == value)
            {
                return;
            }

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
            if (viewModelCode == value)
            {
                return;
            }

            viewModelCode = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(HasViewModelCode));
        }
    }

    public string? MarkdownDocument
    {
        get => markdownDocument;
        set
        {
            if (markdownDocument == value)
            {
                return;
            }

            markdownDocument = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(HasMarkdownDocument));
        }
    }

    public bool StartOnMarkdownDocument
    {
        get => startOnMarkdownDocument;
        set
        {
            if (startOnMarkdownDocument == value)
            {
                return;
            }

            startOnMarkdownDocument = value;
            RaisePropertyChanged();
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

        LoadAndRenderMarkdownDocumentIfPossible(sampleLocation, classViewName);
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    private void LoadAndRenderMarkdownDocumentIfPossible(
        DirectoryInfo sampleLocation,
        string classViewName)
    {
        MarkdownDocument = null;
        StartOnMarkdownDocument = false;
        if (markdownDocumentsFiles is null)
        {
            PrepareReadmeReferences();
        }

        if (markdownDocumentsFiles is null)
        {
            return;
        }

        var className = classViewName.EndsWith(classViewName, StringComparison.Ordinal)
            ? classViewName[..^4]
            : classViewName;

        var docSection = sampleLocation.Name.Replace("SamplesWpf", string.Empty, StringComparison.Ordinal);

        var markdownFile = FindMarkdownFile(Path.Combine("docs", docSection, className)) ??
                           FindMarkdownFile(className + "_Readme");

        if (markdownFile is null)
        {
            var type = FindCustomTypeByName(classViewName) ??
                       FindCustomTypeByName(className);

            if (type?.FullName is not null)
            {
                var sa = type.FullName.Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (sa.Length > 2)
                {
                    var classFolder = sa[^2];
                    markdownFile = FindMarkdownFile(Path.Combine(classFolder, "@Readme"));
                    if (markdownFile is null && classFolder.EndsWith('s'))
                    {
                        classFolder = classFolder[..^1];
                        markdownFile = FindMarkdownFile(Path.Combine(classFolder, "@Readme"));
                    }

                    if (markdownFile is not null && classFolder == "ValueConverters")
                    {
                        StartOnMarkdownDocument = true;
                    }
                }
            }

            markdownFile ??= FindMarkdownFile(classViewName + "_Readme");
        }
        else
        {
            StartOnMarkdownDocument = true;
        }

        if (markdownFile is null)
        {
            return;
        }

        MarkdownDocument = FileHelper.ReadAllText(markdownFile);
    }

    private FileInfo? FindMarkdownFile(string endPath)
    {
        if (!endPath.EndsWith(".md", StringComparison.Ordinal))
        {
            endPath += ".md";
        }

        return markdownDocumentsFiles!.FirstOrDefault(x => x.FullName.EndsWith(endPath, StringComparison.OrdinalIgnoreCase));
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

            type = exportedTypes.FirstOrDefault(x => x.Name.Equals(className, StringComparison.Ordinal) &&
                                                     !x.FullName!.Contains("Models", StringComparison.Ordinal) &&
                                                     !x.FullName!.Contains("XUnitTestTypes", StringComparison.Ordinal));

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

        markdownDocumentsFiles = FileHelper.GetFiles(basePath, "*.md").ToArray();
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