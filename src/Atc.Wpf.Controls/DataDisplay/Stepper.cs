namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A container control that guides users through multi-step processes with
/// numbered step indicators, completion states, connecting lines, and programmatic navigation.
/// </summary>
[ContentProperty(nameof(Items))]
[TemplatePart(Name = "PART_ItemsHost", Type = typeof(StackPanel))]
[SuppressMessage("Design", "MA0051:Method is too long", Justification = "Grid layout construction requires sequential setup.")]
public sealed partial class Stepper : Control
{
    private StackPanel? itemsHost;

    /// <summary>
    /// The orientation of the stepper (Horizontal or Vertical).
    /// </summary>
    [DependencyProperty(
        DefaultValue = Orientation.Horizontal,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private Orientation orientation;

    /// <summary>
    /// The zero-based index of the currently active step.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 0,
        PropertyChangedCallback = nameof(OnActiveStepIndexChanged))]
    private int activeStepIndex;

    /// <summary>
    /// The diameter of the step indicator circle.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 32d,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private double indicatorSize;

    /// <summary>
    /// The brush used for the active step indicator.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private Brush? activeBrush;

    /// <summary>
    /// The brush used for completed step indicators.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private Brush? completedBrush;

    /// <summary>
    /// The brush used for pending step indicators.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private Brush? pendingBrush;

    /// <summary>
    /// The brush used for error step indicators.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private Brush? errorBrush;

    /// <summary>
    /// The brush used for connector lines between steps.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private Brush? lineBrush;

    /// <summary>
    /// The brush used for connector lines between completed steps.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private Brush? completedLineBrush;

    /// <summary>
    /// The thickness of connector lines.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 2d,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private double lineThickness;

    /// <summary>
    /// Additional spacing between steps.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 0d,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private double stepSpacing;

    /// <summary>
    /// Gets the collection of stepper items.
    /// </summary>
    public ObservableCollection<StepperItem> Items { get; } = [];

    /// <summary>
    /// Occurs after the active step has changed.
    /// </summary>
    public event EventHandler<StepperStepChangedEventArgs>? StepChanged;

    /// <summary>
    /// Occurs before the active step changes. Can be canceled.
    /// </summary>
    public event EventHandler<StepperStepChangingEventArgs>? StepChanging;

    static Stepper()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Stepper),
            new FrameworkPropertyMetadata(typeof(Stepper)));
    }

    public Stepper()
    {
        Items.CollectionChanged += OnItemsCollectionChanged;
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Navigates to the previous step.
    /// </summary>
    public void Previous()
    {
        GoToStep(ActiveStepIndex - 1);
    }

    /// <summary>
    /// Navigates to the next step.
    /// </summary>
    public void Next()
    {
        GoToStep(ActiveStepIndex + 1);
    }

    /// <summary>
    /// Navigates to a specific step by index.
    /// </summary>
    /// <param name="index">The zero-based step index to navigate to.</param>
    public void GoToStep(int index)
    {
        if (index < 0 || index >= Items.Count || index == ActiveStepIndex)
        {
            return;
        }

        var changingArgs = new StepperStepChangingEventArgs(ActiveStepIndex, index);
        StepChanging?.Invoke(this, changingArgs);

        if (changingArgs.Cancel)
        {
            return;
        }

        var oldIndex = ActiveStepIndex;
        ActiveStepIndex = index;
        StepChanged?.Invoke(this, new StepperStepChangedEventArgs(oldIndex, index));
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
        UpdateItemMetadata();
        RebuildVisualTree();
    }

    private static void OnLayoutPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Stepper stepper)
        {
            stepper.RebuildVisualTree();
        }
    }

    private static void OnActiveStepIndexChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Stepper stepper)
        {
            stepper.UpdateItemStatuses();
            stepper.RebuildVisualTree();
        }
    }

    private void UpdateItemMetadata()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].StepIndex = i;
            Items[i].IsLast = i == Items.Count - 1;
        }
    }

    private void UpdateItemStatuses()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];

            // Preserve Error status — do not override it
            if (item.Status == StepperStepStatus.Error)
            {
                continue;
            }

            if (i < ActiveStepIndex)
            {
                item.Status = StepperStepStatus.Completed;
            }
            else if (i == ActiveStepIndex)
            {
                item.Status = StepperStepStatus.Active;
            }
            else
            {
                item.Status = StepperStepStatus.Pending;
            }
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

        UpdateItemMetadata();

        if (Orientation == Orientation.Horizontal)
        {
            BuildHorizontalLayout();
        }
        else
        {
            BuildVerticalLayout();
        }
    }

    private void BuildHorizontalLayout()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var grid = CreateHorizontalStepGrid(item);

            if (i > 0 && StepSpacing > 0)
            {
                grid.Margin = new Thickness(StepSpacing, 0, 0, 0);
            }

            itemsHost!.Children.Add(grid);
        }
    }

    private void BuildVerticalLayout()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var grid = CreateVerticalStepGrid(item);

            if (i > 0 && StepSpacing > 0)
            {
                grid.Margin = new Thickness(0, StepSpacing, 0, 0);
            }

            itemsHost!.Children.Add(grid);
        }
    }

    private Grid CreateHorizontalStepGrid(StepperItem item)
    {
        var grid = new Grid();

        // Row 0: indicator + connector line
        // Row 1: title + subtitle
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Col 0: indicator circle
        // Col 1: connector line (star fills remaining space)
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = item.IsLast ? 0 : 20 });

        // Indicator
        var indicator = CreateIndicator(item);
        Grid.SetRow(indicator, 0);
        Grid.SetColumn(indicator, 0);
        grid.Children.Add(indicator);

        // Connector line (not on last item)
        if (!item.IsLast)
        {
            var line = CreateConnectorLine(item, isVertical: false);
            Grid.SetRow(line, 0);
            Grid.SetColumn(line, 1);
            grid.Children.Add(line);
        }

        // Title + subtitle below
        var labelPanel = CreateLabelPanel(item);
        Grid.SetRow(labelPanel, 1);
        Grid.SetColumn(labelPanel, 0);
        Grid.SetColumnSpan(labelPanel, 2);
        grid.Children.Add(labelPanel);

        return grid;
    }

    private Grid CreateVerticalStepGrid(StepperItem item)
    {
        var grid = new Grid();

        // Row 0: indicator + content
        // Row 1: connector line
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star), MinHeight = item.IsLast ? 0 : 20 });

        // Col 0: indicator / connector
        // Col 1: 8px gap
        // Col 2: content
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8, GridUnitType.Pixel) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        // Indicator
        var indicator = CreateIndicator(item);
        Grid.SetRow(indicator, 0);
        Grid.SetColumn(indicator, 0);
        grid.Children.Add(indicator);

        // Title + subtitle + content to the right
        var contentPanel = CreateVerticalContentPanel(item);
        Grid.SetRow(contentPanel, 0);
        Grid.SetColumn(contentPanel, 2);
        Grid.SetRowSpan(contentPanel, 2);
        grid.Children.Add(contentPanel);

        // Connector line (not on last item)
        if (!item.IsLast)
        {
            var line = CreateConnectorLine(item, isVertical: true);
            Grid.SetRow(line, 1);
            Grid.SetColumn(line, 0);
            grid.Children.Add(line);
        }

        return grid;
    }

    private Border CreateIndicator(StepperItem item)
    {
        var size = IndicatorSize;
        var brush = GetIndicatorBrush(item);

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

        var content = GetIndicatorContent(item);
        border.Child = content;

        return border;
    }

    private UIElement GetIndicatorContent(StepperItem item)
    {
        // Custom icon content takes priority
        if (item.IconContent is not null || item.IconTemplate is not null)
        {
            return new ContentPresenter
            {
                Content = item.IconContent,
                ContentTemplate = item.IconTemplate,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
        }

        var text = item.Status switch
        {
            StepperStepStatus.Completed => "\u2713",
            StepperStepStatus.Error => "!",
            _ => (item.StepIndex + 1).ToString(CultureInfo.InvariantCulture),
        };

        var foreground = item.Status switch
        {
            StepperStepStatus.Pending => Foreground,
            _ => Brushes.White,
        };

        var fontSize = item.Status switch
        {
            StepperStepStatus.Completed or StepperStepStatus.Error => IndicatorSize * 0.45,
            _ => IndicatorSize * 0.4,
        };

        return new TextBlock
        {
            Text = text,
            Foreground = foreground,
            FontSize = fontSize,
            FontWeight = FontWeights.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center,
        };
    }

    private Brush GetIndicatorBrush(StepperItem item)
        => item.Status switch
        {
            StepperStepStatus.Active => ActiveBrush ?? Brushes.DodgerBlue,
            StepperStepStatus.Completed => CompletedBrush ?? Brushes.DodgerBlue,
            StepperStepStatus.Error => ErrorBrush ?? Brushes.Red,
            _ => PendingBrush ?? Brushes.Gray,
        };

    private Line CreateConnectorLine(
        StepperItem item,
        bool isVertical)
    {
        // Determine connector color: if the item and the next item are both completed,
        // use the completed line brush; otherwise use the default line brush.
        var isCompletedSpan = item.Status == StepperStepStatus.Completed;
        var brush = isCompletedSpan
            ? (CompletedLineBrush ?? ActiveBrush ?? Brushes.DodgerBlue)
            : (LineBrush ?? Brushes.Gray);

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
            line.MinHeight = 20;
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
            line.MinWidth = 20;
            line.Margin = new Thickness(4, 0, 4, 0);
        }

        return line;
    }

    private StackPanel CreateLabelPanel(StepperItem item)
    {
        var panel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 6, 0, 0),
        };

        if (item.Title is not null)
        {
            var titleBlock = new TextBlock
            {
                Text = item.Title,
                FontWeight = item.Status == StepperStepStatus.Active ? FontWeights.SemiBold : FontWeights.Normal,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
            };

            panel.Children.Add(titleBlock);
        }

        if (item.Subtitle is not null)
        {
            var subtitleBlock = new TextBlock
            {
                Text = item.Subtitle,
                Foreground = Brushes.Gray,
                FontSize = 11,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
            };

            panel.Children.Add(subtitleBlock);
        }

        return panel;
    }

    private StackPanel CreateVerticalContentPanel(StepperItem item)
    {
        var panel = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Top,
        };

        if (item.Title is not null)
        {
            var titleBlock = new TextBlock
            {
                Text = item.Title,
                FontWeight = item.Status == StepperStepStatus.Active ? FontWeights.SemiBold : FontWeights.Normal,
                VerticalAlignment = VerticalAlignment.Center,
            };

            panel.Children.Add(titleBlock);
        }

        if (item.Subtitle is not null)
        {
            var subtitleBlock = new TextBlock
            {
                Text = item.Subtitle,
                Foreground = Brushes.Gray,
                FontSize = 11,
            };

            panel.Children.Add(subtitleBlock);
        }

        if (item.Content is not null)
        {
            var contentPresenter = new ContentPresenter
            {
                Content = item.Content,
                ContentTemplate = item.ContentTemplate,
                Margin = new Thickness(0, 4, 0, 0),
            };

            panel.Children.Add(contentPresenter);
        }

        return panel;
    }
}