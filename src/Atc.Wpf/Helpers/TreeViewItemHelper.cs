namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="TreeViewItem"/>, such as the style of its expander toggle button.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class TreeViewItemHelper
{
    /// <summary>Identifies the ToggleButtonStyle attached property.</summary>
    public static readonly DependencyProperty ToggleButtonStyleProperty = DependencyProperty.RegisterAttached(
        "ToggleButtonStyle",
        typeof(Style),
        typeof(TreeViewItemHelper),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>Gets the value of the ToggleButtonStyle attached property.</summary>
    public static Style? GetToggleButtonStyle(UIElement element)
        => (Style?)element.GetValue(ToggleButtonStyleProperty);

    /// <summary>Sets the value of the ToggleButtonStyle attached property.</summary>
    public static void SetToggleButtonStyle(
        UIElement element,
        Style? value)
        => element.SetValue(ToggleButtonStyleProperty, value);
}