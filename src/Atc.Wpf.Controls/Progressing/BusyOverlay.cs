// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
namespace Atc.Wpf.Controls.Progressing;

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
            BusyContent = $"{Miscellaneous.PleaseWait}...";
        }
    }

    protected bool IsContentVisible { get; set; }

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