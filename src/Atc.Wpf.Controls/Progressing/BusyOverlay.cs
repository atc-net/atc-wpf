// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
using SharedMisc = Atc.Wpf.Resources.Miscellaneous;

namespace Atc.Wpf.Controls.Progressing;

/// <summary>A content control that dims its content and shows busy content while <see cref="IsBusy"/> is <see langword="true"/>.</summary>
[TemplateVisualState(Name = Internal.VisualStates.StateIdle, GroupName = Internal.VisualStates.GroupBusyStatus)]
[TemplateVisualState(Name = Internal.VisualStates.StateBusy, GroupName = Internal.VisualStates.GroupBusyStatus)]
[TemplateVisualState(Name = Internal.VisualStates.StateVisible, GroupName = Internal.VisualStates.GroupVisibility)]
[TemplateVisualState(Name = Internal.VisualStates.StateHidden, GroupName = Internal.VisualStates.GroupVisibility)]
[StyleTypedProperty(Property = "OverlayStyle", StyleTargetType = typeof(Rectangle))]
[StyleTypedProperty(Property = "ProgressBarStyle", StyleTargetType = typeof(ProgressBar))]
public partial class BusyOverlay : ContentControl
{
    private readonly DispatcherTimer displayAfterTimer = new();

    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnIsBusyChanged))]
    private bool isBusy;

    [DependencyProperty]
    private object? busyContent;

    [DependencyProperty]
    private DataTemplate? busyContentTemplate;

    [DependencyProperty]
    private object? busyContentBefore;

    [DependencyProperty]
    private DataTemplate? busyContentTemplateBefore;

    [DependencyProperty]
    private object? busyContentAfter;

    [DependencyProperty]
    private DataTemplate? busyContentTemplateAfter;

    [DependencyProperty(DefaultValue = "TimeSpan.FromSeconds(0.1)")]
    private TimeSpan displayAfter;

    [DependencyProperty]
    private Control? focusAfterBusy;

    [DependencyProperty]
    private Style? overlayStyle;

    [DependencyProperty]
    private Style? progressBarStyle;

    static BusyOverlay()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(BusyOverlay),
            new FrameworkPropertyMetadata(typeof(BusyOverlay)));
    }

    /// <summary>Initializes a new instance of the <see cref="BusyOverlay"/> class.</summary>
    public BusyOverlay()
    {
        displayAfterTimer.Tick += DisplayAfterTimerElapsed;

        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        if (BusyContent is string)
        {
            BusyContent = $"{SharedMisc.PleaseWait}...";
        }
    }

    /// <summary>Gets or sets a value indicating whether the busy content is currently shown.</summary>
    protected bool IsContentVisible { get; set; }

    /// <summary>Called when <see cref="IsBusy"/> changes; shows or hides the busy content and updates the visual state.</summary>
    protected virtual void OnIsBusyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (IsBusy)
        {
            if (DisplayAfter.Equals(TimeSpan.Zero))
            {
                IsContentVisible = true;
            }
            else
            {
                displayAfterTimer.Interval = DisplayAfter;
                displayAfterTimer.Start();
            }
        }
        else
        {
            displayAfterTimer.Stop();
            IsContentVisible = false;

            _ = FocusAfterBusy?.Dispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new Action(() => FocusAfterBusy.Focus()));
        }

        ChangeVisualState(useTransitions: true);
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        ChangeVisualState(useTransitions: false);
    }

    private static void OnIsBusyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((BusyOverlay)d).OnIsBusyChanged(e);

    private void DisplayAfterTimerElapsed(
        object? sender,
        EventArgs e)
    {
        displayAfterTimer.Stop();
        IsContentVisible = true;
        ChangeVisualState(useTransitions: true);
    }

    /// <summary>Moves the control to the visual states that match the current busy and visibility state.</summary>
    protected virtual void ChangeVisualState(bool useTransitions)
    {
        VisualStateManager.GoToState(
            this,
            IsBusy ? Internal.VisualStates.StateBusy : Internal.VisualStates.StateIdle,
            useTransitions);
        VisualStateManager.GoToState(
            this,
            IsContentVisible ? Internal.VisualStates.StateVisible : Internal.VisualStates.StateHidden,
            useTransitions);
    }
}