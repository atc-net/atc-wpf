// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
namespace Atc.Wpf.Controls.Progressing;

[TemplateVisualState(Name = Internal.VisualStates.StateIdle, GroupName = Internal.VisualStates.GroupBusyStatus)]
[TemplateVisualState(Name = Internal.VisualStates.StateBusy, GroupName = Internal.VisualStates.GroupBusyStatus)]
[TemplateVisualState(Name = Internal.VisualStates.StateVisible, GroupName = Internal.VisualStates.GroupVisibility)]
[TemplateVisualState(Name = Internal.VisualStates.StateHidden, GroupName = Internal.VisualStates.GroupVisibility)]
[StyleTypedProperty(Property = "OverlayStyle", StyleTargetType = typeof(Rectangle))]
[StyleTypedProperty(Property = "ProgressBarStyle", StyleTargetType = typeof(ProgressBar))]
public class BusyOverlay : ContentControl
{
    private readonly DispatcherTimer displayAfterTimer = new();

    public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
        nameof(IsBusy),
        typeof(bool),
        typeof(BusyOverlay),
        new PropertyMetadata(
            defaultValue: BooleanBoxes.FalseBox,
            OnIsBusyChanged));

    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public static readonly DependencyProperty BusyContentProperty = DependencyProperty.Register(
        nameof(BusyContent),
        typeof(object),
        typeof(BusyOverlay),
        new PropertyMetadata(propertyChangedCallback: null));

    public object BusyContent
    {
        get => GetValue(BusyContentProperty);
        set => SetValue(BusyContentProperty, value);
    }

    public static readonly DependencyProperty BusyContentTemplateProperty = DependencyProperty.Register(
        nameof(BusyContentTemplate),
        typeof(DataTemplate),
        typeof(BusyOverlay),
        new PropertyMetadata(propertyChangedCallback: null));

    public DataTemplate BusyContentTemplate
    {
        get => (DataTemplate)GetValue(BusyContentTemplateProperty);
        set => SetValue(BusyContentTemplateProperty, value);
    }

    public static readonly DependencyProperty DisplayAfterProperty = DependencyProperty.Register(
        nameof(DisplayAfter),
        typeof(TimeSpan),
        typeof(BusyOverlay),
        new PropertyMetadata(TimeSpan.FromSeconds(0.1)));

    public TimeSpan DisplayAfter
    {
        get => (TimeSpan)GetValue(DisplayAfterProperty);
        set => SetValue(DisplayAfterProperty, value);
    }

    public static readonly DependencyProperty FocusAfterBusyProperty = DependencyProperty.Register(
        nameof(FocusAfterBusy),
        typeof(Control),
        typeof(BusyOverlay),
        new PropertyMetadata(propertyChangedCallback: null));

    public Control FocusAfterBusy
    {
        get => (Control)GetValue(FocusAfterBusyProperty);
        set => SetValue(FocusAfterBusyProperty, value);
    }

    public static readonly DependencyProperty OverlayStyleProperty = DependencyProperty.Register(
        nameof(OverlayStyle),
        typeof(Style),
        typeof(BusyOverlay),
        new PropertyMetadata(propertyChangedCallback: null));

    public Style OverlayStyle
    {
        get => (Style)GetValue(OverlayStyleProperty);
        set => SetValue(OverlayStyleProperty, value);
    }

    public static readonly DependencyProperty ProgressBarStyleProperty = DependencyProperty.Register(
        nameof(ProgressBarStyle),
        typeof(Style),
        typeof(BusyOverlay),
        new PropertyMetadata(propertyChangedCallback: null));

    public Style ProgressBarStyle
    {
        get => (Style)GetValue(ProgressBarStyleProperty);
        set => SetValue(ProgressBarStyleProperty, value);
    }

    static BusyOverlay()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyOverlay), new FrameworkPropertyMetadata(typeof(BusyOverlay)));
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

    protected virtual void OnIsBusyChanged(
        DependencyPropertyChangedEventArgs e)
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
                new Action(() =>
                    {
                        FocusAfterBusy.Focus();
                    }));
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

    private void DisplayAfterTimerElapsed(object? sender, EventArgs e)
    {
        displayAfterTimer.Stop();
        IsContentVisible = true;
        ChangeVisualState(useTransitions: true);
    }

    protected virtual void ChangeVisualState(
        bool useTransitions)
    {
        VisualStateManager.GoToState(this, IsBusy ? Internal.VisualStates.StateBusy : Internal.VisualStates.StateIdle, useTransitions);
        VisualStateManager.GoToState(this, IsContentVisible ? Internal.VisualStates.StateVisible : Internal.VisualStates.StateHidden, useTransitions);
    }
}