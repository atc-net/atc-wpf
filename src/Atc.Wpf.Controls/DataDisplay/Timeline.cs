namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A container control that displays chronological events in a vertical or horizontal
/// layout with customizable dots, connecting lines, and alternating content placement.
/// </summary>
[ContentProperty(nameof(Items))]
[TemplatePart(Name = "PART_ItemsHost", Type = typeof(StackPanel))]
public sealed partial class Timeline : Control
{
    private StackPanel? itemsHost;

    /// <summary>
    /// The orientation of the timeline (vertical or horizontal).
    /// </summary>
    [DependencyProperty(
        DefaultValue = Orientation.Vertical,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private Orientation orientation;

    /// <summary>
    /// The content placement mode (Left, Right, or Alternate).
    /// </summary>
    [DependencyProperty(
        DefaultValue = TimelineMode.Left,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private TimelineMode mode;

    /// <summary>
    /// The default brush used to fill dots.
    /// Individual items can override this via <see cref="TimelineItem.DotBrush"/>.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private Brush? dotBrush;

    /// <summary>
    /// The default diameter of dots.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 12d,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private double dotSize;

    /// <summary>
    /// The default brush used for connecting lines.
    /// Individual items can override this via <see cref="TimelineItem.LineStroke"/>.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private Brush? lineBrush;

    /// <summary>
    /// The thickness of connecting lines.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 2d,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private double lineThickness;

    /// <summary>
    /// The dash style for connecting lines.
    /// </summary>
    [DependencyProperty(
        DefaultValue = TimelineConnectorStyle.Solid,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private TimelineConnectorStyle connectorStyle;

    /// <summary>
    /// The spacing between timeline items.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 20d,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private double itemSpacing;

    /// <summary>
    /// Gets the collection of timeline items.
    /// </summary>
    public ObservableCollection<TimelineItem> Items { get; } = [];

    static Timeline()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Timeline),
            new FrameworkPropertyMetadata(typeof(Timeline)));
    }

    public Timeline()
    {
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
        UpdateIsLastFlags();
        RebuildVisualTree();
    }

    private static void OnLayoutPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Timeline timeline)
        {
            timeline.RebuildVisualTree();
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

        UpdateIsLastFlags();

        if (Orientation == Orientation.Vertical)
        {
            BuildVerticalLayout();
        }
        else
        {
            BuildHorizontalLayout();
        }
    }

    private void BuildVerticalLayout()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var isLeft = DetermineIsLeft(item, i);
            var isAlternate = Mode == TimelineMode.Alternate;
            var grid = CreateVerticalItemGrid(item, isLeft, isAlternate);

            if (i > 0)
            {
                grid.Margin = new Thickness(0, ItemSpacing, 0, 0);
            }

            itemsHost!.Children.Add(grid);
        }
    }

    private void BuildHorizontalLayout()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var grid = CreateHorizontalItemGrid(item);

            if (i > 0)
            {
                grid.Margin = new Thickness(ItemSpacing, 0, 0, 0);
            }

            itemsHost!.Children.Add(grid);
        }
    }

    private bool DetermineIsLeft(
        TimelineItem item,
        int index)
    {
        if (item.Position != TimelineItemPosition.Default)
        {
            return item.Position == TimelineItemPosition.Left;
        }

        return Mode switch
        {
            TimelineMode.Left => true,
            TimelineMode.Right => false,
            TimelineMode.Alternate => index % 2 == 0,
            _ => true,
        };
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "Grid layout construction requires sequential setup.")]
    private Grid CreateVerticalItemGrid(
        TimelineItem item,
        bool isLeft,
        bool isAlternate)
    {
        var grid = new Grid();

        AddVerticalColumnDefinitions(grid, isLeft, isAlternate);

        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star), MinHeight = item.IsLast ? 0 : 8 });

        var dot = CreateDot(item);
        var dotColumn = isAlternate ? 2 : (isLeft ? 2 : 0);
        Grid.SetColumn(dot, dotColumn);
        Grid.SetRow(dot, 0);
        grid.Children.Add(dot);

        var contentPresenter = CreateContentPresenter(item);
        if (isAlternate)
        {
            AddAlternateContent(grid, contentPresenter, item, isLeft);
        }
        else
        {
            var contentColumn = isLeft ? 0 : 2;
            contentPresenter.HorizontalAlignment = isLeft ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            Grid.SetColumn(contentPresenter, contentColumn);
            Grid.SetRow(contentPresenter, 0);
            Grid.SetRowSpan(contentPresenter, 2);
            grid.Children.Add(contentPresenter);
        }

        if (!item.IsLast)
        {
            var line = CreateConnectingLine(item, isVertical: true);
            Grid.SetColumn(line, dotColumn);
            Grid.SetRow(line, 1);
            grid.Children.Add(line);
        }

        return grid;
    }

    private static void AddVerticalColumnDefinitions(
        Grid grid,
        bool isLeft,
        bool isAlternate)
    {
        if (isAlternate)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }
        else if (isLeft)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        }
        else
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }
    }

    private static void AddAlternateContent(
        Grid grid,
        ContentPresenter contentPresenter,
        TimelineItem item,
        bool isLeft)
    {
        var contentColumn = isLeft ? 0 : 4;
        contentPresenter.HorizontalAlignment = isLeft ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        Grid.SetColumn(contentPresenter, contentColumn);
        Grid.SetRow(contentPresenter, 0);
        Grid.SetRowSpan(contentPresenter, 2);
        grid.Children.Add(contentPresenter);

        if (item.OppositeContent is not null)
        {
            var oppositePresenter = CreateOppositeContentPresenter(item);
            var oppositeColumn = isLeft ? 4 : 0;
            oppositePresenter.HorizontalAlignment = isLeft ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            Grid.SetColumn(oppositePresenter, oppositeColumn);
            Grid.SetRow(oppositePresenter, 0);
            Grid.SetRowSpan(oppositePresenter, 2);
            grid.Children.Add(oppositePresenter);
        }
    }

    private Grid CreateHorizontalItemGrid(TimelineItem item)
    {
        var grid = new Grid();

        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(8, GridUnitType.Pixel) });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = item.IsLast ? 0 : 8 });

        var dot = CreateDot(item);
        Grid.SetColumn(dot, 0);
        Grid.SetRow(dot, 0);
        grid.Children.Add(dot);

        var contentPresenter = CreateContentPresenter(item);
        contentPresenter.HorizontalAlignment = HorizontalAlignment.Left;
        Grid.SetColumn(contentPresenter, 0);
        Grid.SetColumnSpan(contentPresenter, 2);
        Grid.SetRow(contentPresenter, 2);
        grid.Children.Add(contentPresenter);

        if (!item.IsLast)
        {
            var line = CreateConnectingLine(item, isVertical: false);
            Grid.SetColumn(line, 1);
            Grid.SetRow(line, 0);
            grid.Children.Add(line);
        }

        return grid;
    }

    private Border CreateDot(TimelineItem item)
    {
        var size = item.DotSize > 0 ? item.DotSize : DotSize;
        var brush = item.DotBrush ?? DotBrush;

        var border = new Border
        {
            Width = size,
            Height = size,
            CornerRadius = new CornerRadius(size / 2),
            Background = brush,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            SnapsToDevicePixels = true,
        };

        if (item.DotContent is not null || item.DotTemplate is not null)
        {
            var presenter = new ContentPresenter
            {
                Content = item.DotContent,
                ContentTemplate = item.DotTemplate,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            border.Child = presenter;
        }

        return border;
    }

    private Line CreateConnectingLine(
        TimelineItem item,
        bool isVertical)
    {
        var brush = item.LineStroke ?? LineBrush;
        var line = new Line
        {
            Stroke = brush,
            StrokeThickness = LineThickness,
            SnapsToDevicePixels = true,
        };

        if (isVertical)
        {
            line.X1 = 0;
            line.X2 = 0;
            line.Y1 = 0;
            line.Y2 = 1;
            line.Stretch = Stretch.Fill;
            line.HorizontalAlignment = HorizontalAlignment.Center;
            line.VerticalAlignment = VerticalAlignment.Stretch;
            line.MinHeight = 8;
        }
        else
        {
            line.X1 = 0;
            line.X2 = 1;
            line.Y1 = 0;
            line.Y2 = 0;
            line.Stretch = Stretch.Fill;
            line.HorizontalAlignment = HorizontalAlignment.Stretch;
            line.VerticalAlignment = VerticalAlignment.Center;
            line.MinWidth = 8;
        }

        var dashArray = GetDashArray();
        if (dashArray is not null)
        {
            line.StrokeDashArray = dashArray;
        }

        return line;
    }

    private DoubleCollection? GetDashArray()
        => ConnectorStyle switch
        {
            TimelineConnectorStyle.Dashed => [4, 2],
            TimelineConnectorStyle.Dotted => [1, 2],
            _ => null,
        };

    private static ContentPresenter CreateContentPresenter(TimelineItem item)
        => new()
        {
            Content = item.Content,
            ContentTemplate = item.ContentTemplate,
            VerticalAlignment = VerticalAlignment.Center,
        };

    private static ContentPresenter CreateOppositeContentPresenter(
        TimelineItem item)
        => new()
        {
            Content = item.OppositeContent,
            ContentTemplate = item.OppositeContentTemplate,
            VerticalAlignment = VerticalAlignment.Center,
        };
}