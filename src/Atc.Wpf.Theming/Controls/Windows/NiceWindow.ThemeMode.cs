namespace Atc.Wpf.Theming.Controls.Windows;

public partial class NiceWindow
{
    /// <summary>
    /// When the application sets .NET's <c>Application.ThemeMode</c>, WPF gives every window whose
    /// <see cref="FrameworkElement.Style"/> is <see langword="null"/> the Fluent <see cref="Window"/> style, as a
    /// resource reference to <c>typeof(Window)</c>: when the window is created (a window built in code resolves its
    /// implicit style later) and again each time the theme mode changes. Its template replaces the NiceWindow
    /// template (title bar, window buttons), so that reference is removed and the window keeps its own style.
    /// A style set by the application is kept.
    /// </summary>
    private void RemoveFluentWindowStyle()
    {
        if (ReadLocalValue(StyleProperty) is not Expression expression)
        {
            return;
        }

        var converter = TypeDescriptor.GetConverter(expression);
        if (converter.CanConvertTo(typeof(MarkupExtension)) &&
            converter.ConvertTo(expression, typeof(MarkupExtension)) is DynamicResourceExtension { ResourceKey: Type resourceKey } &&
            resourceKey == typeof(Window))
        {
            ClearValue(StyleProperty);
        }
    }
}