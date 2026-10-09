namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="ToggleButton"/>, such as the content direction.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class ToggleButtonHelper
{
    /// <summary>Identifies the ContentDirection attached property.</summary>
    public static readonly DependencyProperty ContentDirectionProperty = DependencyProperty.RegisterAttached(
        "ContentDirection",
        typeof(FlowDirection),
        typeof(ToggleButtonHelper),
        new FrameworkPropertyMetadata(FlowDirection.LeftToRight));

    /// <summary>Gets the value of the ContentDirection attached property.</summary>
    public static FlowDirection GetContentDirection(UIElement element)
        => (FlowDirection)element.GetValue(ContentDirectionProperty);

    /// <summary>Sets the value of the ContentDirection attached property.</summary>
    public static void SetContentDirection(
        UIElement element,
        FlowDirection value)
        => element.SetValue(ContentDirectionProperty, value);
}