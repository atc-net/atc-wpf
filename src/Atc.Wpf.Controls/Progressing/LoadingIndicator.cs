// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Controls.Progressing;

[TemplatePart(Name = TemplateBorderName, Type = typeof(Border))]
public sealed partial class LoadingIndicator : Control
{
    internal const string TemplateBorderName = "PART_Border";

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:Field names should begin with lower-case letter", Justification = "OK.")]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
    private Border? PART_Border;

    [DependencyProperty(
        PropertyChangedCallback = nameof(OnCustomColorBrushChanged))]
    private Brush? customColorBrush;

    [DependencyProperty(
        DefaultValue = 1,
        PropertyChangedCallback = nameof(OnSpeedRatioChanged))]
    private double speedRatio;

    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnIsActiveChanged))]
    private bool isActive;

    [DependencyProperty(
        DefaultValue = LoadingIndicatorType.ArcsRing)]
    private LoadingIndicatorType mode;

    static LoadingIndicator()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(LoadingIndicator),
            new FrameworkPropertyMetadata(typeof(LoadingIndicator)));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (CustomColorBrush is not null)
        {
            SetCurrentValue(ForegroundProperty, CustomColorBrush);
        }

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

    private static void OnCustomColorBrushChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var loadingIndicator = (LoadingIndicator)d;
        if (e.NewValue is Brush newBrush)
        {
            loadingIndicator.SetCurrentValue(ForegroundProperty, newBrush);
        }
        else
        {
            loadingIndicator.ClearValue(ForegroundProperty);
        }
    }

    private static void OnSpeedRatioChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var loadingIndicator = (LoadingIndicator)d;

        if (loadingIndicator.PART_Border == null || !loadingIndicator.IsActive)
        {
            return;
        }

        SetStoryBoardSpeedRatio(loadingIndicator.PART_Border, (double)e.NewValue);
    }

    private static void OnIsActiveChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var loadingIndicator = (LoadingIndicator)d;

        if (loadingIndicator.PART_Border == null)
        {
            return;
        }

        if (!(bool)e.NewValue)
        {
            VisualStateManager.GoToElementState(
                loadingIndicator.PART_Border,
                IndicatorVisualStateNames.InactiveState.Name,
                useTransitions: false);
            loadingIndicator.PART_Border.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
        }
        else
        {
            VisualStateManager.GoToElementState(
                loadingIndicator.PART_Border,
                IndicatorVisualStateNames.ActiveState.Name,
                useTransitions: false);

            loadingIndicator.PART_Border.SetCurrentValue(VisibilityProperty, Visibility.Visible);

            SetStoryBoardSpeedRatio(loadingIndicator.PART_Border, loadingIndicator.SpeedRatio);
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