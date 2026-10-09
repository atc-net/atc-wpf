namespace Atc.Wpf.MarkupExtensions;

/// <summary>
/// Markup Extension: StaticResource.
/// A markup extension that supports static (XAML load time) resource references made from XAML.
/// </summary>
[MarkupExtensionReturnType(typeof(object))]
[Localizability(LocalizationCategory.NeverLocalize)]
public sealed class StaticResourceExtension : System.Windows.StaticResourceExtension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StaticResourceExtension"/> class.
    /// </summary>
    public StaticResourceExtension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticResourceExtension"/> class with the specified resource key.
    /// </summary>
    /// <param name="resourceKey">The key of the resource to reference.</param>
    public StaticResourceExtension(object resourceKey)
        : base(resourceKey)
    {
    }
}