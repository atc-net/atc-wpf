namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A horizontal control for switching between mutually exclusive options,
/// similar to iOS UISegmentedControl or Material Design toggle buttons.
/// </summary>
[ContentProperty(nameof(Items))]
[TemplatePart(Name = "PART_ItemsHost", Type = typeof(Panel))]
public sealed partial class Segmented : Control
{
    private Panel? itemsHost;
    private bool isUpdatingSelection;

    /// <summary>
    /// The index of the currently selected segment (-1 means none).
    /// </summary>
    [DependencyProperty(
        DefaultValue = -1,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnSelectedIndexChanged))]
    private int selectedIndex;

    /// <summary>
    /// The currently selected <see cref="SegmentedItem"/>, or null if none.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedItemChanged))]
    private SegmentedItem? selectedItem;

    /// <summary>
    /// The corner radius of the outer pill shape.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private CornerRadius cornerRadius;

    /// <summary>
    /// The background brush applied to the selected segment.
    /// </summary>
    [DependencyProperty]
    private Brush? selectedBackground;

    /// <summary>
    /// The foreground brush applied to the selected segment.
    /// </summary>
    [DependencyProperty]
    private Brush? selectedForeground;

    /// <summary>
    /// The background brush shown when hovering over an unselected segment.
    /// </summary>
    [DependencyProperty]
    private Brush? hoverBackground;

    /// <summary>
    /// The brush used for vertical separators between segments.
    /// </summary>
    [DependencyProperty]
    private Brush? separatorBrush;

    /// <summary>
    /// Whether all segments should share equal width.
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private bool equalSegmentWidth;

    /// <summary>
    /// Occurs when the selected segment changes.
    /// </summary>
    public event EventHandler<SegmentedSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Gets the collection of segments.
    /// </summary>
    public ObservableCollection<SegmentedItem> Items { get; } = [];

    static Segmented()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Segmented),
            new FrameworkPropertyMetadata(typeof(Segmented)));
    }

    public Segmented()
    {
        CornerRadius = new CornerRadius(4);
        Items.CollectionChanged += OnItemsCollectionChanged;
        Loaded += OnLoaded;
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        itemsHost = GetTemplateChild("PART_ItemsHost") as Panel;
        RebuildVisualTree();
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnKeyDown(e);

        if (e.Handled || Items.Count == 0)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Left:
                MoveSelection(-1);
                e.Handled = true;
                break;
            case Key.Right:
                MoveSelection(1);
                e.Handled = true;
                break;
            case Key.Home:
                SelectFirstEnabled();
                e.Handled = true;
                break;
            case Key.End:
                SelectLastEnabled();
                e.Handled = true;
                break;
        }
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
        if (e.OldItems is not null)
        {
            foreach (SegmentedItem item in e.OldItems)
            {
                item.Click -= OnItemClick;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (SegmentedItem item in e.NewItems)
            {
                item.Click += OnItemClick;
            }
        }

        UpdateItemMetadata();
        RebuildVisualTree();
    }

    private static void OnSelectedIndexChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Segmented control)
        {
            control.UpdateSelection((int)e.OldValue, (int)e.NewValue);
        }
    }

    private static void OnSelectedItemChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Segmented control && !control.isUpdatingSelection)
        {
            var newItem = e.NewValue as SegmentedItem;
            var newIndex = newItem is null ? -1 : control.Items.IndexOf(newItem);
            control.SelectedIndex = newIndex;
        }
    }

    private static void OnLayoutPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Segmented control)
        {
            control.RebuildVisualTree();
        }
    }

    private void UpdateItemMetadata()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].Index = i;
            Items[i].IsFirst = i == 0;
            Items[i].IsLast = i == Items.Count - 1;
        }
    }

    private void UpdateSelection(
        int oldIndex,
        int newIndex)
    {
        if (isUpdatingSelection)
        {
            return;
        }

        isUpdatingSelection = true;

        try
        {
            var oldItem = oldIndex >= 0 && oldIndex < Items.Count ? Items[oldIndex] : null;
            var newItem = newIndex >= 0 && newIndex < Items.Count ? Items[newIndex] : null;

            for (var i = 0; i < Items.Count; i++)
            {
                Items[i].IsSelected = i == newIndex;
            }

            SelectedItem = newItem;

            RebuildVisualTree();

            SelectionChanged?.Invoke(
                this,
                new SegmentedSelectionChangedEventArgs(oldIndex, newIndex, oldItem, newItem));
        }
        finally
        {
            isUpdatingSelection = false;
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

        if (EqualSegmentWidth && itemsHost is Grid grid)
        {
            grid.ColumnDefinitions.Clear();
            for (var i = 0; i < Items.Count; i++)
            {
                // Column for the item
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Column for separator (except after last item)
                if (i < Items.Count - 1)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                }
            }

            var colIndex = 0;
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                ApplyCornerRadius(item, i);
                Grid.SetColumn(item, colIndex);
                grid.Children.Add(item);
                colIndex++;

                if (i < Items.Count - 1)
                {
                    var separator = CreateSeparator(i);
                    Grid.SetColumn(separator, colIndex);
                    grid.Children.Add(separator);
                    colIndex++;
                }
            }
        }
        else if (itemsHost is StackPanel stackPanel)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                ApplyCornerRadius(item, i);
                stackPanel.Children.Add(item);

                if (i < Items.Count - 1)
                {
                    stackPanel.Children.Add(CreateSeparator(i));
                }
            }
        }
    }

    private void ApplyCornerRadius(
        SegmentedItem item,
        int itemIndex)
    {
        var cr = CornerRadius;

        if (Items.Count == 1)
        {
            item.Tag = cr;
        }
        else if (itemIndex == 0)
        {
            item.Tag = new CornerRadius(cr.TopLeft, 0, 0, cr.BottomLeft);
        }
        else if (itemIndex == Items.Count - 1)
        {
            item.Tag = new CornerRadius(0, cr.TopRight, cr.BottomRight, 0);
        }
        else
        {
            item.Tag = new CornerRadius(0);
        }
    }

    private Border CreateSeparator(int beforeIndex)
    {
        var isAdjacentToSelected = SelectedIndex == beforeIndex || SelectedIndex == beforeIndex + 1;

        return new Border
        {
            Width = 1,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0, 4, 0, 4),
            Background = isAdjacentToSelected
                ? Brushes.Transparent
                : (SeparatorBrush ?? Brushes.Gray),
        };
    }

    private void OnItemClick(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is SegmentedItem item)
        {
            var index = Items.IndexOf(item);
            if (index >= 0)
            {
                SelectedIndex = index;
            }
        }
    }

    private void MoveSelection(int direction)
    {
        var current = SelectedIndex;
        var next = current + direction;

        while (next >= 0 && next < Items.Count)
        {
            if (Items[next].IsEnabled)
            {
                SelectedIndex = next;
                return;
            }

            next += direction;
        }
    }

    private void SelectFirstEnabled()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            if (Items[i].IsEnabled)
            {
                SelectedIndex = i;
                return;
            }
        }
    }

    private void SelectLastEnabled()
    {
        for (var i = Items.Count - 1; i >= 0; i--)
        {
            if (Items[i].IsEnabled)
            {
                SelectedIndex = i;
                return;
            }
        }
    }
}