namespace Atc.Wpf.Extensions;

/// <summary>
/// Markup Extension used to dynamically set the Language property of an Markup element to the
/// the current <see cref="CultureManager.UiCulture" /> property value.
/// </summary>
/// <remarks>
/// The culture used for displaying data bound items is based on the Language property.  This
/// extension allows you to dynamically change the language based on the current <see cref="CultureManager.UiCulture" />.
/// </remarks>
[MarkupExtensionReturnType(typeof(XmlLanguage))]
public class UiCultureExtension : ManagedMarkupExtension
{
    /// <summary>
    /// List of active extensions.
    /// </summary>
    private static readonly MarkupExtensionManager MarkupExtensionManager = new MarkupExtensionManager(2);

    /// <summary>
    /// Initializes a new instance of the <see cref="UiCultureExtension" /> class.
    /// </summary>
    public UiCultureExtension()
        : base(MarkupExtensionManager)
    {
    }

    /// <summary>
    /// Gets the MarkupManager for this extension.
    /// </summary>
    public static MarkupExtensionManager MarkupManager => MarkupExtensionManager;

    /// <summary>
    /// Use the Markup Manager to update all targets.
    /// </summary>
    public static void UpdateAllTargets()
    {
        MarkupExtensionManager.UpdateAllTargets();
    }

    /// <summary>
    /// Return the <see cref="XmlLanguage" /> to use for the associated Markup element.
    /// </summary>
    /// <returns>
    /// The <see cref="XmlLanguage" /> corresponding to the current.
    /// <see cref="CultureManager.UiCulture" /> property value.
    /// </returns>
    protected override object GetValue()
    {
        return XmlLanguage.GetLanguage(CultureManager.UiCulture.IetfLanguageTag);
    }
}