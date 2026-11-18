// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// View model for the sample viewer, which shows the selected sample together with its XAML, code-behind, view model code and readme.
/// </summary>
public sealed partial class SampleViewerViewModel : ViewModelBase
{
    private const string Error = "Error";
    private const int ReadmeTabIndex = 4;

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleViewerViewModel"/> class and subscribes to <see cref="SampleItemMessage"/>.
    /// </summary>
    public SampleViewerViewModel()
    {
        Messenger.Default.Register<SampleItemMessage>(
            this,
            SampleItemMessageHandler);

        copySampleReferenceCommand = new RelayCommand(
            CopySampleReference,
            () => SampleReference is not null);
    }

    private readonly RelayCommand copySampleReferenceCommand;

    private FileInfo[]? markdownDocumentsFiles;

    /// <summary>
    /// Gets or sets the index of the selected tab in the sample viewer.
    /// </summary>
    [ObservableProperty]
    private int tabSelectedIndex;

    /// <summary>
    /// Gets or sets the header of the selected sample.
    /// </summary>
    [ObservableProperty]
    private string? header;

    /// <summary>
    /// Gets a text that points to the selected sample (its type, header and XAML file),
    /// ready to paste into a chat with an AI agent; <see langword="null"/> when no sample is selected.
    /// </summary>
    [ObservableProperty(
        SetterAccessModifier = PropertyAccessModifier.Private,
        AfterChangedCallback = "copySampleReferenceCommand.RaiseCanExecuteChanged();")]
    private string? sampleReference;

    /// <summary>
    /// Gets or sets the instance of the selected sample view.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(HasSampleContent)])]
    private UserControl? sampleContent;

    /// <summary>
    /// Gets or sets the XAML source of the selected sample.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(HasXamlCode)])]
    private string? xamlCode;

    /// <summary>
    /// Gets or sets the code-behind source of the selected sample.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(HasCodeBehindCode)])]
    private string? codeBehindCode;

    /// <summary>
    /// Gets or sets the view model source of the selected sample.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(HasViewModelCode)])]
    private string? viewModelCode;

    /// <summary>
    /// Gets or sets the markdown content of the readme found for the selected sample.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(HasMarkdownDocument)])]
    private string? markdownDocument;

    /// <summary>
    /// Gets or sets a value indicating whether the readme tab is selected when the sample is shown.
    /// </summary>
    [ObservableProperty]
    private bool startOnMarkdownDocument;

    /// <summary>
    /// Gets a value indicating whether <see cref="SampleContent"/> is set.
    /// </summary>
    public bool HasSampleContent => SampleContent is not null;

    /// <summary>
    /// Gets a value indicating whether <see cref="XamlCode"/> is set.
    /// </summary>
    public bool HasXamlCode => XamlCode is not null;

    /// <summary>
    /// Gets a value indicating whether <see cref="CodeBehindCode"/> is set.
    /// </summary>
    public bool HasCodeBehindCode => CodeBehindCode is not null;

    /// <summary>
    /// Gets a value indicating whether <see cref="ViewModelCode"/> is set.
    /// </summary>
    public bool HasViewModelCode => ViewModelCode is not null;

    /// <summary>
    /// Gets a value indicating whether <see cref="MarkdownDocument"/> is set.
    /// </summary>
    public bool HasMarkdownDocument => MarkdownDocument is not null;

    /// <summary>
    /// Gets the command that copies <see cref="SampleReference"/> to the clipboard.
    /// </summary>
    public ICommand CopySampleReferenceCommand => copySampleReferenceCommand;

    /// <summary>
    /// Creates the text that points to a sample: its type, its header in the sample app and its XAML file.
    /// </summary>
    /// <param name="sampleHeader">The header of the sample in the sample app.</param>
    /// <param name="sampleTypeFullName">The full name of the sample view type.</param>
    /// <param name="sampleXamlPath">The path of the sample XAML file, or <see langword="null"/> to leave it out.</param>
    /// <returns>The sample reference.</returns>
    public static string CreateSampleReference(
        string sampleHeader,
        string sampleTypeFullName,
        string? sampleXamlPath)
        => string.IsNullOrEmpty(sampleXamlPath)
            ? $"For the sample {sampleTypeFullName} (\"{sampleHeader}\" in the sample app)"
            : $"For the sample {sampleTypeFullName} (\"{sampleHeader}\" in the sample app, {sampleXamlPath})";

    private void SampleItemMessageHandler(SampleItemMessage obj)
    {
        if (string.IsNullOrEmpty(obj.SampleItemPath))
        {
            ClearSelectedViewData();
        }
        else
        {
            if (obj.Header is not null)
            {
                SetSelectedViewData(
                    obj.Header,
                    obj.SampleItemPath);
            }
        }

        // Show the readme when the sample asks for it, otherwise the first enabled tab
        TabSelectedIndex = StartOnMarkdownDocument
            ? ReadmeTabIndex
            : GetFirstEnabledTabIndex();
    }

    private int GetFirstEnabledTabIndex()
    {
        if (HasSampleContent)
        {
            return 0;
        }

        if (HasXamlCode)
        {
            return 1;
        }

        if (HasCodeBehindCode)
        {
            return 2;
        }

        if (HasViewModelCode)
        {
            return 3;
        }

        if (HasMarkdownDocument)
        {
            return ReadmeTabIndex;
        }

        return 0;
    }

    private void ClearSelectedViewData()
    {
        TryDisposeAndClearSampleContent();

        Header = null;
        SampleReference = null;
        XamlCode = null;
        CodeBehindCode = null;
        ViewModelCode = null;
        MarkdownDocument = null;
        StartOnMarkdownDocument = false;
    }

    private void TryDisposeAndClearSampleContent()
    {
        if (SampleContent is null)
        {
            return;
        }

        if (SampleContent.DataContext is IDisposable disposableDataContext)
        {
            disposableDataContext.Dispose();
        }
        else if (SampleContent.GetType() != SampleContent.DataContext?.GetType() &&
                 SampleContent is IDisposable disposableControl)
        {
            disposableControl.Dispose();
        }

        SampleContent = null;
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    private void SetSelectedViewData(
        string sampleHeader,
        string samplePath)
    {
        var entryAssembly = Assembly.GetEntryAssembly()!;
        var sampleType = GetTypeBySamplePath(
            entryAssembly,
            samplePath);

        if (sampleType is null)
        {
            _ = MessageBox.Show(
                $"Can't find sample by path '{samplePath}'",
                Error,
                MessageBoxButton.OK);
            return;
        }

        if (Activator.CreateInstance(sampleType) is not UserControl instance)
        {
            MessageBox.Show(
                $"Can't create instance of sample by path '{samplePath}'",
                Error,
                MessageBoxButton.OK);
            return;
        }

        var sampleTypeAssemblyLocation = new DirectoryInfo(Path.GetDirectoryName(sampleType.Assembly.Location)!);
        var baseLocation = ExtractBasePath(sampleTypeAssemblyLocation);
        if (baseLocation is null)
        {
            MessageBox.Show(
                "Can't find sample by invalid base location",
                Error,
                MessageBoxButton.OK);
            return;
        }

        var classViewName = ExtractClassName(instance.ToString()!);
        var sampleLocation = ExtractSamplePath(baseLocation, classViewName, sampleType);
        if (sampleLocation is null)
        {
            MessageBox.Show(
                "Can't find sample by invalid location",
                Error,
                MessageBoxButton.OK);
            return;
        }

        TryDisposeAndClearSampleContent();

        Header = sampleHeader;
        SampleContent = instance;
        var xamlFilePath = Path.Combine(sampleLocation.FullName, classViewName + ".xaml");
        SampleReference = CreateSampleReference(
            sampleHeader,
            sampleType.FullName!,
            File.Exists(xamlFilePath) ? GetSolutionRelativePath(xamlFilePath) : null);
        XamlCode = ReadFileText(xamlFilePath);
        CodeBehindCode = ReadFileText(Path.Combine(sampleLocation.FullName, classViewName + ".xaml.cs"));

        if (instance.DataContext is null ||
            nameof(SampleViewerViewModel).Equals(
                ExtractClassName(instance.DataContext.ToString()!),
                StringComparison.Ordinal))
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

        var className = MarkdownFileLookup.GetClassName(classViewName);

        var docSection = sampleLocation.Name.Replace("SamplesWpf", string.Empty, StringComparison.Ordinal);

        var markdownFile = FindMarkdownFile(Path.Combine("docs", docSection, className));
        if (markdownFile is null)
        {
            markdownFile = FindMarkdownFile(className + "_Readme");
        }
        else
        {
            StartOnMarkdownDocument = true;
        }

        if (markdownFile is null)
        {
            var type = FindCustomTypeByName(classViewName) ??
                       FindCustomTypeByName(className);

            if (type?.FullName is not null)
            {
                var sa = type.FullName.Split(
                    '.',
                    StringSplitOptions.RemoveEmptyEntries);
                if (sa.Length > 2)
                {
                    var classFolder = sa[^2];
                    markdownFile = FindMarkdownFile(
                        Path.Combine(
                            classFolder,
                            "@Readme"));
                    if (markdownFile is null && classFolder.EndsWith('s'))
                    {
                        classFolder = classFolder[..^1];
                        markdownFile = FindMarkdownFile(
                            Path.Combine(
                                classFolder,
                                "@Readme"));
                    }

                    if (markdownFile is not null && classFolder == "ValueConverters")
                    {
                        StartOnMarkdownDocument = true;
                    }
                }
            }

            markdownFile ??= FindMarkdownFile(classViewName + "_Readme");
        }

        if (markdownFile is null)
        {
            return;
        }

        MarkdownDocument = FileHelper.ReadAllText(markdownFile);
    }

    private FileInfo? FindMarkdownFile(string endPath)
        => MarkdownFileLookup.Find(markdownDocumentsFiles!, endPath);

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
            .OrderBy(
                x => x.FullName,
                StringComparer.Ordinal);

        foreach (var customAssembly in customAssemblies)
        {
            var exportedTypes = customAssembly.GetExportedTypes();

            type = exportedTypes.FirstOrDefault(x => x.Name.Equals(
                                                         className,
                                                         StringComparison.Ordinal) &&
                                                     !x.FullName!.Contains(
                                                         "Models",
                                                         StringComparison.Ordinal) &&
                                                     !x.FullName!.Contains(
                                                         "XUnitTestTypes",
                                                         StringComparison.Ordinal));

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

        var indexOf = assemblyLocation.IndexOf(
            "\\sample",
            StringComparison.OrdinalIgnoreCase);
        var baseLocation = indexOf == -1
            ? assemblyLocation
            : assemblyLocation[..indexOf];

        return baseLocation;
    }

    private static string GetSolutionRelativePath(string filePath)
    {
        var basePath = GetBasePath();
        return Directory.Exists(basePath)
            ? Path.GetRelativePath(basePath, filePath).Replace('\\', '/')
            : filePath;
    }

    private void CopySampleReference()
        => ClipboardHelper.TrySetText(SampleReference);

    /// <summary>
    /// Scans the solution folder for markdown files and caches them for readme lookup.
    /// </summary>
    public void PrepareReadmeReferences()
    {
        var basePath = GetBasePath();

        markdownDocumentsFiles = FileHelper
            .GetFiles(
                basePath,
                "*.md")
            .ToArray();
    }

    private static Type? GetTypeBySamplePath(
        Assembly entryAssembly,
        string samplePath)
    {
        var sampleType = entryAssembly
            .GetExportedTypes()
            .FirstOrDefault(x => x.FullName is not null && x.FullName.EndsWith(
                samplePath,
                StringComparison.Ordinal));

        if (sampleType is not null)
        {
            return sampleType;
        }

        var assemblyStartName = entryAssembly
            .GetName()
            .Name!
            .Split('.')[0];
        foreach (var assembly in AppDomain.CurrentDomain
                     .GetAssemblies()
                     .Where(x => !x.IsDynamic &&
                                 x.FullName!.StartsWith(
                                     assemblyStartName,
                                     StringComparison.Ordinal)))
        {
            sampleType = assembly
                .GetExportedTypes()
                .FirstOrDefault(x => x.FullName is not null && x.FullName.EndsWith(
                    samplePath,
                    StringComparison.Ordinal));

            if (sampleType is not null)
            {
                return sampleType;
            }
        }

        return null;
    }

    [SuppressMessage("Performance", "MA0098:Use indexer instead of LINQ methods", Justification = "OK.")]
    private static string ExtractClassName(string classFullName)
        => classFullName
            .Split('.')
            .Last();

    private static DirectoryInfo? ExtractBasePath(DirectoryInfo path)
    {
        if ("bin".Equals(
                path.Name,
                StringComparison.Ordinal))
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
        => SampleFileIndex
            .For(baseLocation.FullName)
            .FindFolder(classViewName, sampleType.FullName!);

    private static string? ReadFileText(string filePath)
        => File.Exists(filePath)
            ? File.ReadAllText(filePath)
            : null;
}