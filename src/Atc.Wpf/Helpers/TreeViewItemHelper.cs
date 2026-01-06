namespace Atc.Wpf.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class TreeViewItemHelper
{
    public static readonly DependencyProperty ToggleButtonStyleProperty = DependencyProperty.RegisterAttached(
        "ToggleButtonStyle",
        typeof(Style),
        typeof(TreeViewItemHelper),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static Style? GetToggleButtonStyle(UIElement element)
        => (Style?)element.GetValue(ToggleButtonStyleProperty);

    public static void SetToggleButtonStyle(
        UIElement element,
        Style? value)
        => element.SetValue(ToggleButtonStyleProperty, value);
}