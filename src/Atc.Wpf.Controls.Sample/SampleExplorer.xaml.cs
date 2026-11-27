namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// The whole sample-app shell: a filter box, a tab per <see cref="SampleCategory"/> (with a badge counting the
/// samples that match the filter), the sample trees, expand/collapse all, and the <see cref="SampleViewerView"/>
/// with right-to-left, language and theme selectors above it and a status bar below.
/// </summary>
[ContentProperty(nameof(Categories))]
public partial class SampleExplorer
{
    private const string TreeViewStyleKey = "AtcApps.Styles.TreeView";
    private const string TreeViewItemStyleKey = "AtcApps.Styles.TreeViewItem";

    private readonly List<TreeView> sampleTreeViews = [];
    private readonly List<TabItem> categoryTabs = [];
    private readonly List<Badge> categoryBadges = [];
    private bool isBuilt;
    private bool suppressTabSelectionChanged;

    /// <summary>
    /// The text shown in the status bar.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnStatusTextChanged))]
    private string? statusText;

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleExplorer"/> class.
    /// </summary>
    public SampleExplorer()
    {
        InitializeComponent();

        Categories.CollectionChanged += OnCategoriesChanged;
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Gets the categories: one tab and one sample tree each, in this order.
    /// </summary>
    public ObservableCollection<SampleCategory> Categories { get; } = [];

    /// <summary>
    /// Gets the sample trees created for the <see cref="Categories"/>, in the same order (empty until loaded).
    /// </summary>
    public IReadOnlyList<TreeView> SampleTreeViews => sampleTreeViews;

    /// <summary>
    /// Gets the sample viewer that shows the selected sample.
    /// </summary>
    public SampleViewerView Viewer => SampleViewer;

    /// <summary>
    /// Gets the category tabs, in the order of the <see cref="Categories"/>.
    /// </summary>
    internal IReadOnlyList<TabItem> CategoryTabs => categoryTabs;

    /// <summary>
    /// Gets the badges of the category tabs, in the order of the <see cref="Categories"/>.
    /// </summary>
    internal IReadOnlyList<Badge> CategoryBadges => categoryBadges;

    /// <summary>
    /// Gets or sets how a selected sample is announced; by default to the <see cref="SampleViewerView"/>
    /// through <see cref="Messenger.Default"/>.
    /// </summary>
    internal Action<SampleItemMessage> SendSampleItemMessage { get; set; } = message => Messenger.Default.Send(message);

    /// <summary>
    /// Creates the tabs and sample trees for the <see cref="Categories"/>, replacing any made before.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// A category has no key, a key is used twice, or a tree type is not a <see cref="TreeView"/> with a public
    /// parameterless constructor.
    /// </exception>
    internal void BuildCategories()
    {
        var trees = Categories.Select(CreateTree).ToList();
        var duplicateKey = Categories
            .GroupBy(x => x.Key, StringComparer.Ordinal)
            .FirstOrDefault(x => x.Count() > 1)?
            .Key;
        if (duplicateKey is not null)
        {
            throw new InvalidOperationException($"The sample category key '{duplicateKey}' is used more than once; keys name the automation IDs and must be unique.");
        }

        foreach (var tree in sampleTreeViews)
        {
            tree.SelectedItemChanged -= TreeViewOnSelectedItemChanged;
        }

        sampleTreeViews.Clear();
        categoryTabs.Clear();
        categoryBadges.Clear();
        TreePanel.Children.Clear();

        // Looked up now (on load), not in the constructor: the Atc styles may be merged above the explorer.
        AddTreeItemStyles();

        suppressTabSelectionChanged = true;
        CategoryTabControl.Items.Clear();
        for (var i = 0; i < Categories.Count; i++)
        {
            AddCategory(Categories[i], trees[i]);
        }

        CategoryTabControl.SelectedIndex = -1;
        suppressTabSelectionChanged = false;

        isBuilt = true;
        UpdateTreeVisibility();
        ApplyFilter();
    }

    private static void OnStatusTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((SampleExplorer)d).StatusTextBlock.Text = e.NewValue as string;

    private static TreeView CreateTree(SampleCategory category)
    {
        if (string.IsNullOrWhiteSpace(category.Key))
        {
            throw new InvalidOperationException($"The sample category '{category.Header}' has no key; the key names its automation IDs.");
        }

        if (category.TreeType is null ||
            !typeof(TreeView).IsAssignableFrom(category.TreeType) ||
            category.TreeType.GetConstructor(Type.EmptyTypes) is null)
        {
            throw new InvalidOperationException($"The tree type '{category.TreeType?.Name ?? "(none)"}' of the sample category '{category.Key}' must be a TreeView with a public parameterless constructor.");
        }

        return (TreeView)Activator.CreateInstance(category.TreeType)!;
    }

    private void AddTreeItemStyles()
    {
        var baseStyle = TryFindResource(TreeViewItemStyleKey) as Style;

        var categoryItemStyle = new Style(typeof(TreeViewItem), baseStyle);
        categoryItemStyle.Setters.Add(new Setter(TreeViewItem.IsExpandedProperty, value: true));
        categoryItemStyle.Setters.Add(new Setter(Control.FontWeightProperty, FontWeights.Bold));
        categoryItemStyle.Setters.Add(new Setter(Control.ForegroundProperty, new DynamicResourceExtension("AtcApps.Brushes.ThemeForeground4")));
        TreePanel.Resources[typeof(TreeViewItem)] = categoryItemStyle;

        var sampleItemStyle = new Style(typeof(SampleTreeViewItem), baseStyle);
        sampleItemStyle.Setters.Add(new Setter(TreeViewItem.IsExpandedProperty, value: true));
        sampleItemStyle.Setters.Add(new Setter(Control.FontWeightProperty, FontWeights.Normal));
        sampleItemStyle.Setters.Add(new Setter(Control.ForegroundProperty, new DynamicResourceExtension("AtcApps.Brushes.ThemeForeground")));
        TreePanel.Resources[typeof(SampleTreeViewItem)] = sampleItemStyle;
    }

    private void AddCategory(
        SampleCategory category,
        TreeView tree)
    {
        AutomationProperties.SetAutomationId(tree, SampleExplorerIds.Tree(category.Key));
        tree.BorderThickness = new Thickness(0);
        if (TryFindResource(TreeViewStyleKey) is Style treeViewStyle)
        {
            tree.Style = treeViewStyle;
        }

        tree.SelectedItemChanged += TreeViewOnSelectedItemChanged;
        TreePanel.Children.Add(tree);
        sampleTreeViews.Add(tree);

        var badge = new Badge
        {
            Margin = new Thickness(5, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
            BadgePlacementMode = BadgePlacementMode.Right,
        };
        categoryBadges.Add(badge);

        var tab = new TabItem
        {
            Width = 200,
            Tag = category.Key,
            Header = CreateTabHeader(category.Header, badge),
        };
        AutomationProperties.SetAutomationId(tab, SampleExplorerIds.Tab(category.Key));
        CategoryTabControl.Items.Add(tab);
        categoryTabs.Add(tab);
    }

    private static GridEx CreateTabHeader(
        string header,
        Badge badge)
    {
        var grid = new GridEx { Columns = "*,Auto" };
        BindToTabHeader(grid, TextElement.FontFamilyProperty, nameof(FontFamily));
        BindToTabHeader(grid, TextElement.FontSizeProperty, nameof(FontSize));
        BindToTabHeader(grid, TextElement.FontWeightProperty, nameof(FontWeight));
        BindToTabHeader(grid, TextElement.ForegroundProperty, "(TextElement.Foreground)");

        var textBlock = new TextBlock { Text = header };
        Grid.SetColumn(textBlock, 0);
        Grid.SetColumn(badge, 1);
        grid.Children.Add(textBlock);
        grid.Children.Add(badge);
        return grid;
    }

    private static void BindToTabHeader(
        FrameworkElement target,
        DependencyProperty property,
        string path)
        => target.SetBinding(
            property,
            new Binding(path)
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ContentControlEx), 1),
            });

    private void OnCategoriesChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (isBuilt)
        {
            BuildCategories();
        }
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (!isBuilt)
        {
            BuildCategories();
        }

        Keyboard.Focus(FilterTextBox);
    }

    private void TabControlOnSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (suppressTabSelectionChanged || !ReferenceEquals(e.OriginalSource, CategoryTabControl))
        {
            return;
        }

        UpdateTreeVisibility();
        ApplyFilter();
    }

    private void TreeViewOnSelectedItemChanged(
        object sender,
        RoutedPropertyChangedEventArgs<object> e)
    {
        var sampleItem = e.NewValue as SampleTreeViewItem;
        SendSampleItemMessage(new SampleItemMessage(
            sampleItem?.Header?.ToString(),
            sampleItem?.SamplePath));

        if (sampleItem is not null &&
            sender is TreeView treeView)
        {
            var index = sampleTreeViews.IndexOf(treeView);
            if (index >= 0)
            {
                categoryTabs[index].IsSelected = true;
            }
        }
    }

    private void FilterOnTextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        if (!isBuilt)
        {
            return;
        }

        suppressTabSelectionChanged = true;
        CategoryTabControl.SelectedIndex = -1;
        suppressTabSelectionChanged = false;

        UpdateTreeVisibility();
        foreach (var tree in sampleTreeViews)
        {
            ClearSelection(tree.Items);
        }

        ApplyFilter();
    }

    private void ExpandAllOnClick(
        object sender,
        RoutedEventArgs e)
        => SetExpanded(isExpanded: true);

    private void CollapseAllOnClick(
        object sender,
        RoutedEventArgs e)
        => SetExpanded(isExpanded: false);

    private void SetExpanded(bool isExpanded)
    {
        foreach (var tree in sampleTreeViews)
        {
            SampleTreeFilter.SetExpanded(tree.Items, isExpanded);
        }
    }

    private void UpdateTreeVisibility()
    {
        var selectedTab = CategoryTabControl.SelectedItem as TabItem;
        for (var i = 0; i < sampleTreeViews.Count; i++)
        {
            sampleTreeViews[i].Visibility = selectedTab is null || ReferenceEquals(categoryTabs[i], selectedTab)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void ApplyFilter()
    {
        var filter = FilterTextBox.Text;
        for (var i = 0; i < sampleTreeViews.Count; i++)
        {
            SampleTreeFilter.ApplyFilter(sampleTreeViews[i].Items, filter);
            var count = SampleTreeFilter.CountMatches(sampleTreeViews[i].Items, filter);
            categoryBadges[i].BadgeContent = count > 0 ? count : null;
        }
    }

    private static void ClearSelection(ItemCollection items)
    {
        foreach (var treeViewItem in items.OfType<TreeViewItem>())
        {
            treeViewItem.IsSelected = false;
            ClearSelection(treeViewItem.Items);
        }
    }
}