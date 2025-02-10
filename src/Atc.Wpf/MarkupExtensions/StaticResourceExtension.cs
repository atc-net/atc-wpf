namespace Atc.Wpf.MarkupExtensions;

/// <summary>
/// Markup Extension: StaticResource.
/// A markup extension that supports static (XAML load time) resource references made from XAML.
/// </summary>
[MarkupExtensionReturnType(typeof(object))]
[Localizability(LocalizationCategory.NeverLocalize)]
public sealed class StaticResourceExtension : System.Windows.StaticResourceExtension
{
    public StaticResourceExtension()
    {
    }

    public StaticResourceExtension(object resourceKey)
        : base(resourceKey)
    {
    }
}