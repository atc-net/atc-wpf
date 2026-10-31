using JsonViewerStrings = Atc.Wpf.Components.Resources.JsonViewer;

namespace Atc.Wpf.Components.Viewers;

public partial class JsonViewer
{
    private readonly JsonNodeExpansionState expansionState = new();

    [DependencyProperty(DefaultValue = true)]
    private bool showActionAndInformationBar;

    [DependencyProperty(DefaultValue = false)]
    private bool suppressErrorMessages;

    /// <summary>
    /// Whether the root of a loaded document starts expanded, showing its first level. Deeper levels start
    /// collapsed; use <em>Expand all</em> in the context menu to open everything. Default <c>true</c>.
    /// </summary>
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

        // The expand state is kept by node, outside the (virtualized) tree items, which read it when they
        // get their node and report the user's changes back through the Expanded and Collapsed events.
        var itemContainerStyle = new Style(typeof(TreeViewItem), JsonTreeView.ItemContainerStyle);
        itemContainerStyle.Setters.Add(
            new Setter(
                TreeViewItem.IsExpandedProperty,
                new Binding { Converter = expansionState, Mode = BindingMode.OneWay }));
        JsonTreeView.ItemContainerStyle = itemContainerStyle;
        JsonTreeView.AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(OnTreeViewItemExpandedChanged));
        JsonTreeView.AddHandler(TreeViewItem.CollapsedEvent, new RoutedEventHandler(OnTreeViewItemExpandedChanged));

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
    /// Removes the displayed JSON tree.
    /// </summary>
    public void Clear()
    {
        JsonTreeView.ItemsSource = null;
        JsonTreeView.Items.Clear();
        expansionState.Clear();
    }

    /// <summary>
    /// Parses the JSON text and shows it in the tree view. Empty input is ignored; invalid JSON
    /// is reported in a message box unless <c>SuppressErrorMessages</c> is set.
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
        expansionState.Clear();

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

            if (StartExpanded)
            {
                expansionState.Set(node, isExpanded: true);
            }

            JsonTreeView.ItemsSource = children;
        }
        catch (Exception ex)
        {
            if (!SuppressErrorMessages)
            {
                MessageBox.Show(string.Format(
                    CultureInfo.CurrentCulture,
                    JsonViewerStrings.CouldNotOpenJsonStringFormat1,
                    ex.Message));
            }
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

        // The node colours come from JsonColorSchema; re-create the tree items so they pick up the new theme,
        // without parsing the document again (the nodes and their expand state are kept).
        JsonColorSchema.SetThemeMode((ThemeMode)e.NewValue);
        jsonViewer.RefreshTreeItems();
    }

    private void RefreshTreeItems()
    {
        if (JsonTreeView.ItemsSource is not null)
        {
            JsonTreeView.Items.Refresh();
        }
    }

    private void OnTreeViewItemExpandedChanged(
        object sender,
        RoutedEventArgs e)
    {
        if (e.OriginalSource is TreeViewItem { DataContext: JsonNode node } treeViewItem)
        {
            expansionState.Set(node, treeViewItem.IsExpanded);
        }
    }

    private static void OnDataChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs args)
    {
        if (d is not JsonViewer jsonViewer)
        {
            return;
        }

        var jsonData = args.NewValue?.ToString();
        if (string.IsNullOrEmpty(jsonData))
        {
            jsonViewer.Clear();
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

        // Set the state by node (the tree is virtualized, so most nodes have no tree item), then let the tree
        // items read it again.
        if (isExpanded)
        {
            expansionState.ExpandAll(JsonTreeView.Items.OfType<JsonNode>());
        }
        else
        {
            expansionState.Clear();
        }

        RefreshTreeItems();
    }
}