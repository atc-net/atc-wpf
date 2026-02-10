namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A container control that guides users through multi-step processes with
/// numbered step indicators, completion states, connecting lines, and programmatic navigation.
/// </summary>
[ContentProperty(nameof(Items))]
[TemplatePart(Name = "PART_ItemsHost", Type = typeof(Panel))]
public sealed partial class Stepper : Control
{
    private Panel? itemsHost;

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
    /// Whether clicking a step indicator navigates to that step.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isClickable;

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

        CommandBindings.Add(new CommandBinding(
            StepperCommands.StepClickedCommand,
            OnStepClickedExecuted,
            OnStepClickedCanExecute));
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
        itemsHost = GetTemplateChild("PART_ItemsHost") as Panel;
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

    private void UpdateItemAppearance()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];

            // Indicator text (skip items with custom icon content)
            if (item.IconContent is null && item.IconTemplate is null)
            {
                item.IndicatorText = item.Status switch
                {
                    StepperStepStatus.Completed => "\u2713",
                    StepperStepStatus.Error => "!",
                    _ => (item.StepIndex + 1).ToString(CultureInfo.InvariantCulture),
                };
            }

            // Push resolved brushes based on status
            item.IndicatorBrush = GetIndicatorBrush(item);
            item.ConnectorBrush = item.Status == StepperStepStatus.Completed
                ? (CompletedLineBrush ?? ActiveBrush)
                : LineBrush;

            // Push layout properties
            item.ResolvedIndicatorSize = IndicatorSize;
            item.ResolvedLineThickness = LineThickness;
            item.ResolvedIsClickable = IsClickable;

            // Compute connector margin: base gap (4px) plus half the step spacing on each side
            var halfSpacing = StepSpacing / 2;
            item.ConnectorMargin = Orientation == Orientation.Horizontal
                ? new Thickness(4 + halfSpacing, 0, 4 + halfSpacing, 0)
                : new Thickness(0, 4 + halfSpacing, 0, 4 + halfSpacing);
        }
    }

    private Brush? GetIndicatorBrush(StepperItem item)
        => item.Status switch
        {
            StepperStepStatus.Active => ActiveBrush,
            StepperStepStatus.Completed => CompletedBrush,
            StepperStepStatus.Error => ErrorBrush,
            _ => PendingBrush,
        };

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
        UpdateItemStatuses();
        UpdateItemAppearance();

        var templateKey = Orientation == Orientation.Horizontal
            ? "AtcApps.DataTemplate.StepperItemContainer.Horizontal"
            : "AtcApps.DataTemplate.StepperItemContainer.Vertical";

        var template = TryFindResource(templateKey) as DataTemplate;

        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];

            var presenter = new ContentPresenter
            {
                Content = item,
                ContentTemplate = template,
            };

            itemsHost.Children.Add(presenter);
        }
    }

    private void OnStepClickedExecuted(
        object sender,
        ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is StepperItem item)
        {
            GoToStep(item.StepIndex);
        }
    }

    private void OnStepClickedCanExecute(
        object sender,
        CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = IsClickable;
    }
}