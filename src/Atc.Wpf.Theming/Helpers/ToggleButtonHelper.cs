namespace Atc.Wpf.Theming.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class ToggleButtonHelper
{
    public static readonly DependencyProperty ContentDirectionProperty = DependencyProperty.RegisterAttached(
        "ContentDirection",
        typeof(FlowDirection),
        typeof(ToggleButtonHelper),
        new FrameworkPropertyMetadata(FlowDirection.LeftToRight));

    public static FlowDirection GetContentDirection(
        UIElement element)
        => (FlowDirection)element.GetValue(ContentDirectionProperty);

    public static void SetContentDirection(
        UIElement element,
        FlowDirection value)
        => element.SetValue(ContentDirectionProperty, value);
}