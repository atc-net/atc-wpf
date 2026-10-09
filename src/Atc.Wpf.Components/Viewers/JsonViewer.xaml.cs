using JsonViewerStrings = Atc.Wpf.Components.Resources.JsonViewer;

namespace Atc.Wpf.Components.Viewers;

public partial class JsonViewer
{
    [DependencyProperty(DefaultValue = true)]
    private bool showActionAndInformationBar;

    [DependencyProperty(DefaultValue = false)]
    private bool suppressErrorMessages;

    [DependencyProperty(DefaultValue = true)]
    private bool startExpanded;

    [DependencyProperty(PropertyChangedCallback = nameof(OnDataChanged))]
    private string data;

    [DependencyProperty(
        DefaultValue = ThemeMode.Light,
        PropertyChangedCallback = nameof(OnThemeModeChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private ThemeMode themeMode;

    private string? themeNameWhenUnloaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonViewer"/> class, matching its theme mode to the current application theme.
    /// </summary>
    public JsonViewer()
    {
        InitializeComponent();

        var detectTheme = ThemeManager.Current.DetectTheme();
        if (detectTheme is not null &&
            Enum<ThemeMode>.TryParse(detectTheme.BaseColorScheme, ignoreCase: false, out var themeModeValue))
        {
            ThemeMode = themeModeValue;
        }

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Parses the JSON text and shows it in the tree view. Empty input is ignored; invalid JSON
    /// is reported in a message box (the invalid-format message is skipped when
    /// <c>SuppressErrorMessages</c> is set).
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    public void Load(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        JsonTreeView.ItemsSource = null;
        JsonTreeView.Items.Clear();

        if (!json.IsFormatJson())
        {
            if (!SuppressErrorMessages)
            {
                MessageBox.Show(JsonViewerStrings.InvalidJsonFormat);
            }

            return;
        }

        var children = new List<JsonNode>();

        try
        {
            var node = JsonNode.Parse(json);
            children.Add(node);

            JsonTreeView.ItemsSource = children;
        }
        catch (Exception ex)
        {
            MessageBox.Show(string.Format(
                CultureInfo.CurrentCulture,
                JsonViewerStrings.CouldNotOpenJsonStringFormat1,
                ex.Message));
        }
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        // ThemeManager is process-wide: only listen while in the visual tree, otherwise it keeps this control alive.
        ThemeManager.Current.ThemeChanged -= OnThemeChanged;
        ThemeManager.Current.ThemeChanged += OnThemeChanged;

        // Catch up on a theme change that happened while this control was unloaded (e.g. in a hidden tab).
        var currentTheme = ThemeManager.Current.DetectTheme();
        if (themeNameWhenUnloaded is not null &&
            currentTheme is not null &&
            !string.Equals(currentTheme.Name, themeNameWhenUnloaded, StringComparison.Ordinal))
        {
            ApplyTheme(currentTheme);
        }

        themeNameWhenUnloaded = null;
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        themeNameWhenUnloaded = ThemeManager.Current.DetectTheme()?.Name;
        ThemeManager.Current.ThemeChanged -= OnThemeChanged;
    }

    private void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
        => ApplyTheme(e.NewTheme);

    private void ApplyTheme(Theme theme)
    {
        if (Enum<ThemeMode>.TryParse(theme.BaseColorScheme, ignoreCase: false, out var themeModeValue))
        {
            ThemeMode = themeModeValue;
        }
    }

    private static void OnThemeModeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not JsonViewer jsonViewer)
        {
            return;
        }

        jsonViewer.Load(jsonViewer.Data);
    }

    private static void OnDataChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs args)
    {
        if (args.NewValue is null)
        {
            return;
        }

        var jsonData = args.NewValue.ToString();
        if (string.IsNullOrEmpty(jsonData))
        {
            return;
        }

        if (d is not JsonViewer jsonViewer)
        {
            return;
        }

        jsonViewer.Load(jsonData);
    }

    private void CopyToClipboard(
        object sender,
        RoutedEventArgs e)
        => System.Windows.Clipboard.SetText(
            string.IsNullOrEmpty(Data)
                ? "{ }"
                : Data);

    private void ExpandAll(
        object sender,
        RoutedEventArgs e)
        => ToggleItems(isExpanded: true);

    private void CollapseAll(
        object sender,
        RoutedEventArgs e)
        => ToggleItems(isExpanded: false);

    private void ToggleItems(bool isExpanded)
    {
        if (JsonTreeView.Items.IsEmpty)
        {
            return;
        }

        var prevCursor = Cursor;
        Cursor = Cursors.Wait;
        ToggleItems(JsonTreeView, JsonTreeView.Items, isExpanded);
        Cursor = prevCursor;
    }

    private void ToggleItems(
        ItemsControl parentContainer,
        IEnumerable items,
        bool isExpanded)
    {
        var itemGen = parentContainer.ItemContainerGenerator;
        if (itemGen.Status == GeneratorStatus.ContainersGenerated)
        {
            Recurse(items, isExpanded, itemGen);
        }
        else
        {
            itemGen.StatusChanged += (_, _) => Recurse(items, isExpanded, itemGen);
        }
    }

    private void Recurse(
        IEnumerable items,
        bool isExpanded,
        ItemContainerGenerator itemContainerGenerator)
    {
        if (itemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
        {
            return;
        }

        foreach (var item in items)
        {
            if (itemContainerGenerator.ContainerFromItem(item) is not TreeViewItem tvi)
            {
                continue;
            }

            tvi.IsExpanded = isExpanded;
            ToggleItems(tvi, tvi.Items, isExpanded);
        }
    }
}