namespace Atc.Wpf.Forms.FontEditing.Internal;

/// <summary>
/// Resolves theme-aware default brushes for the font picker controls.
/// Uses SetResourceReference so the brushes track runtime theme changes.
/// Only applies a theme brush when the target DP is still at its metadata default,
/// so external bindings always win.
/// </summary>
internal static class ThemeBrushHelper
{
    private const string ThemeForegroundResourceKey = "AtcApps.Brushes.ThemeForeground";
    private const string ThemeBackgroundResourceKey = "AtcApps.Brushes.ThemeBackground";

    public static void ApplyDefaults(
        FrameworkElement element,
        DependencyProperty foregroundProperty,
        DependencyProperty backgroundProperty)
    {
        ArgumentNullException.ThrowIfNull(element);
        ArgumentNullException.ThrowIfNull(foregroundProperty);
        ArgumentNullException.ThrowIfNull(backgroundProperty);

        TryApply(element, foregroundProperty, ThemeForegroundResourceKey);
        TryApply(element, backgroundProperty, ThemeBackgroundResourceKey);
    }

    private static void TryApply(
        FrameworkElement element,
        DependencyProperty property,
        string resourceKey)
    {
        var source = DependencyPropertyHelper.GetValueSource(element, property);
        if (source.BaseValueSource != BaseValueSource.Default)
        {
            return;
        }

        if (element.TryFindResource(resourceKey) is SolidColorBrush)
        {
            element.SetResourceReference(property, resourceKey);
        }
    }
}