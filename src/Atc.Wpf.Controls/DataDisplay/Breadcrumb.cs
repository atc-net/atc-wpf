namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A navigation control that displays a horizontal trail of clickable breadcrumb items
/// with customizable separators and optional overflow handling.
/// </summary>
[ContentProperty(nameof(Items))]
[TemplatePart(Name = "PART_ItemsHost", Type = typeof(StackPanel))]
public sealed partial class Breadcrumb : Control
{
    private readonly List<BreadcrumbItem> hookedItems = [];
    private StackPanel? itemsHost;

    /// <summary>
    /// The content used as separator between breadcrumb items.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private object separator;

    /// <summary>
    /// An optional template for rendering the separator content.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private DataTemplate? separatorTemplate;

    /// <summary>
    /// Specifies how overflow items are handled.
    /// </summary>
    [DependencyProperty(
        DefaultValue = BreadcrumbOverflowMode.None,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private BreadcrumbOverflowMode overflowMode;

    /// <summary>
    /// The number of trailing items to keep visible when overflow is active.
    /// A value of 0 means all items are visible. The first item is always shown.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 0,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private int maxVisibleItems;

    /// <summary>
    /// Occurs when any breadcrumb item is clicked.
    /// </summary>
    public event EventHandler<BreadcrumbItemClickedEventArgs>? ItemClicked;

    /// <summary>
    /// Gets the collection of breadcrumb items.
    /// </summary>
    public ObservableCollection<BreadcrumbItem> Items { get; } = [];

    static Breadcrumb()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Breadcrumb),
            new FrameworkPropertyMetadata(typeof(Breadcrumb)));
        StyleCheck.Register(typeof(Breadcrumb));
    }

    /// <summary>Initializes a new instance of the <see cref="Breadcrumb"/> class.</summary>
    public Breadcrumb()
    {
        Separator = "/";
        Items.CollectionChanged += OnItemsCollectionChanged;
        Loaded += OnLoaded;
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        itemsHost = GetTemplateChild("PART_ItemsHost") as StackPanel;
        RebuildVisualTree();
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        RebuildVisualTree();
    }

    private void OnItemsCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        // Re-hooked from the current items: a Reset (Clear) carries no old items.
        foreach (var item in hookedItems)
        {
            item.Click -= OnItemClick;
            if (!Items.Contains(item))
            {
                item.IsLast = false;
            }
        }

        hookedItems.Clear();
        foreach (var item in Items)
        {
            item.Click += OnItemClick;
            hookedItems.Add(item);
        }

        UpdateIsLastFlags();
        RebuildVisualTree();
    }

    private static void OnLayoutPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Breadcrumb breadcrumb)
        {
            breadcrumb.RebuildVisualTree();
        }
    }

    private void UpdateIsLastFlags()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].IsLast = i == Items.Count - 1;
        }
    }

    private void RebuildVisualTree()
    {
        if (itemsHost is null)
        {
            return;
        }

        itemsHost.Children.Clear();

        if (Items.Count == 0)
        {
            return;
        }

        var shouldOverflow = OverflowMode == BreadcrumbOverflowMode.Collapsed
                             && MaxVisibleItems > 0
                             && Items.Count > MaxVisibleItems + 1;

        if (shouldOverflow)
        {
            RebuildWithOverflow();
        }
        else
        {
            RebuildWithoutOverflow();
        }
    }

    private void RebuildWithoutOverflow()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            if (i > 0)
            {
                itemsHost!.Children.Add(CreateSeparator());
            }

            itemsHost!.Children.Add(Items[i]);
        }
    }

    private void RebuildWithOverflow()
    {
        // Always show first item
        itemsHost!.Children.Add(Items[0]);
        itemsHost.Children.Add(CreateSeparator());

        // Create overflow button with hidden items
        var hiddenStart = 1;
        var hiddenEnd = Items.Count - MaxVisibleItems;
        itemsHost.Children.Add(CreateOverflowButton(hiddenStart, hiddenEnd));

        // Show the last MaxVisibleItems items
        for (var i = hiddenEnd; i < Items.Count; i++)
        {
            itemsHost.Children.Add(CreateSeparator());
            itemsHost.Children.Add(Items[i]);
        }
    }

    private ContentPresenter CreateSeparator()
    {
        var presenter = new ContentPresenter
        {
            Content = Separator,
            ContentTemplate = SeparatorTemplate,
            Margin = new Thickness(4, 0, 4, 0),
            VerticalAlignment = VerticalAlignment.Center,
        };

        return presenter;
    }

    private Button CreateOverflowButton(
        int startIndex,
        int endIndex)
    {
        var button = new Button
        {
            Content = "\u2026",
            Style = TryFindResource("AtcApps.Styles.Breadcrumb.OverflowButton") as Style,
        };

        var contextMenu = new ContextMenu();
        for (var i = startIndex; i < endIndex; i++)
        {
            var index = i;
            var item = Items[i];
            var menuItem = new MenuItem
            {
                Header = item.Content,
            };

            menuItem.Click += (_, _) =>
            {
                ItemClicked?.Invoke(this, new BreadcrumbItemClickedEventArgs(item, index));

                if (item.Command?.CanExecute(item.CommandParameter) == true)
                {
                    item.Command.Execute(item.CommandParameter);
                }
            };

            contextMenu.Items.Add(menuItem);
        }

        button.Click += (_, _) =>
        {
            contextMenu.PlacementTarget = button;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = true;
        };

        return button;
    }

    private void OnItemClick(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is BreadcrumbItem item)
        {
            var index = Items.IndexOf(item);
            ItemClicked?.Invoke(this, new BreadcrumbItemClickedEventArgs(item, index));
        }
    }
}