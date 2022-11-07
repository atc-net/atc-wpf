// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Controls.Progressing;

[TemplatePart(Name = TemplateBorderName, Type = typeof(Border))]
public class LoadingIndicator : Control
{
    internal const string TemplateBorderName = "PART_Border";

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:Field names should begin with lower-case letter", Justification = "OK.")]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
    private Border? PART_Border;

    public static readonly DependencyProperty SpeedRatioProperty = DependencyProperty.Register(
        nameof(SpeedRatio),
        typeof(double),
        typeof(LoadingIndicator),
        new PropertyMetadata(
            defaultValue: 1d,
            OnSpeedRatioChanged));

    public double SpeedRatio
    {
        get => (double)GetValue(SpeedRatioProperty);
        set => SetValue(SpeedRatioProperty, value);
    }

    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive),
        typeof(bool),
        typeof(LoadingIndicator),
        new PropertyMetadata(
            defaultValue: true,
            OnIsActiveChanged));

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
        nameof(Mode),
        typeof(LoadingIndicatorType),
        typeof(LoadingIndicator),
        new PropertyMetadata(LoadingIndicatorType.ArcsRing));

    public LoadingIndicatorType Mode
    {
        get => (LoadingIndicatorType)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    static LoadingIndicator()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(LoadingIndicator),
            new FrameworkPropertyMetadata(typeof(LoadingIndicator)));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        PART_Border = GetTemplateChild(TemplateBorderName) as Border;

        if (PART_Border == null)
        {
            return;
        }

        VisualStateManager.GoToElementState(
            PART_Border,
            IsActive ? IndicatorVisualStateNames.ActiveState.Name : IndicatorVisualStateNames.InactiveState.Name,
            useTransitions: false);

        SetStoryBoardSpeedRatio(PART_Border, SpeedRatio);

        PART_Border.SetCurrentValue(
            VisibilityProperty,
            IsActive ? Visibility.Visible : Visibility.Collapsed);
    }

    private static void OnSpeedRatioChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var li = (LoadingIndicator)d;

        if (li.PART_Border == null || !li.IsActive)
        {
            return;
        }

        SetStoryBoardSpeedRatio(li.PART_Border, (double)e.NewValue);
    }

    private static void OnIsActiveChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var li = (LoadingIndicator)d;

        if (li.PART_Border == null)
        {
            return;
        }

        if (!(bool)e.NewValue)
        {
            VisualStateManager.GoToElementState(
                li.PART_Border,
                IndicatorVisualStateNames.InactiveState.Name,
                useTransitions: false);
            li.PART_Border.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
        }
        else
        {
            VisualStateManager.GoToElementState(
                li.PART_Border,
                IndicatorVisualStateNames.ActiveState.Name,
                useTransitions: false);

            li.PART_Border.SetCurrentValue(VisibilityProperty, Visibility.Visible);

            SetStoryBoardSpeedRatio(li.PART_Border, li.SpeedRatio);
        }
    }

    private static void SetStoryBoardSpeedRatio(
        FrameworkElement element,
        double speedRatio)
    {
        var activeStates = element.GetActiveVisualStates();
        foreach (var activeState in activeStates)
        {
            activeState.Storyboard.SetSpeedRatio(element, speedRatio);
        }
    }
}