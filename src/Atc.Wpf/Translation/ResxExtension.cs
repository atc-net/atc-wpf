// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Atc.Wpf.Translation;

/// <summary>
/// A markup extension to allow resources for WPF Windows and controls to be retrieved
/// from an embedded resource (resx) file associated with the window or control.
/// </summary>
[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
[MarkupExtensionReturnType(typeof(object))]
public class ResxExtension : ManagedMarkupExtension
{
    /// <summary>
    /// Defines the handling method for the <see cref="Resource" /> event.
    /// </summary>
    public event EventHandler<ResourceEventArgs>? Resource;

    /// <summary>
    /// The resource manager to use for this extension. Holding a strong reference to the
    /// Resource Manager keeps it in the cache while ever there are ResxExtensions that
    /// are using it.
    /// </summary>
    private ResourceManager? resourceManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResxExtension" /> class.
    /// </summary>
    public ResxExtension()
        : base(MarkupManager)
    {
    }

    /// <summary>
         /// Initializes a new instance of the <see cref="ResxExtension" /> class.
         /// </summary>
         /// <param name="resxName">Name of the RESX.</param>
         /// <param name="key">The key.</param>
    public ResxExtension(
        string resxName,
        string key)
        : base(MarkupManager)
    {
        ResxName = resxName;
        Key = key;
        DefaultValue = key;
    }

    /// <summary>
    /// Gets the markup manager.
    /// </summary>
    /// <value>
    /// The markup manager.
    /// </value>
    public static MarkupExtensionManager MarkupManager { get; } = new(40);

    /// <summary>
    /// Gets or sets the fully qualified name of the embedded resx (without .resources) to get
    /// the resource from.
    /// </summary>
    public string? ResxName { get; set; }

    /// <summary>
    /// Gets or sets the name of the resource key.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets the default value to use if the resource can't be found.
    /// </summary>
    /// <remarks>
    /// This particularly useful for properties which require non-null
    /// values because it allows the page to be displayed even if
    /// the resource can't be loaded.
    /// </remarks>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the suffix.
    /// </summary>
    public string? Suffix { get; set; }

    /// <summary>
    /// Use the Markup Manager to update all targets.
    /// </summary>
    public static void UpdateAllTargets()
    {
        MarkupManager.UpdateAllTargets();
    }

    /// <summary>
    /// Update the ResxExtension target with the given key.
    /// </summary>
    /// <param name="key">The key.</param>
    public static void UpdateTarget(
        string key)
    {
        foreach (var target in MarkupManager.ActiveExtensions
                     .Cast<ResxExtension>()
                     .Where(target => target.Key == key))
        {
            target.UpdateTarget();
        }
    }

    /// <summary>
    /// Return the value associated with the key from the resource manager.
    /// </summary>
    /// <returns>The value from the resources if possible otherwise the default value.</returns>
    protected override object GetValue()
    {
        if (string.IsNullOrEmpty(ResxName))
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(Key))
        {
            return string.Empty;
        }

        object? result = null;
        try
        {
            Resource?.Invoke(this, new ResourceEventArgs(ResxName, Key, CultureManager.UiCulture));

            if (result is null)
            {
                resourceManager ??= GlobalizationHelper.GetResourceManager(ResxName);
                result = resourceManager.GetObject(Key, CultureManager.UiCulture);
            }

            if (result is not null &&
                !string.IsNullOrEmpty(Suffix))
            {
                result += Suffix;
            }
        }
        catch
        {
            // Dummy
        }

        return result ?? GetDefaultValue(Key);
    }

    /// <summary>
    /// Return the default value for the property.
    /// </summary>
    private object GetDefaultValue(
        string key)
    {
        object? result = DefaultValue;
        var targetType = TargetPropertyType;
        if (DefaultValue is null)
        {
            if (targetType == typeof(string) ||
                targetType == typeof(object))
            {
                result = "#" + key + Suffix;
            }
        }
        else
        {
            if (targetType is null ||
                targetType == typeof(string) ||
                targetType == typeof(object))
            {
                return result!;
            }

            try
            {
                var tc = TypeDescriptor.GetConverter(targetType);
                result = tc.ConvertFromInvariantString(DefaultValue);
            }
            catch
            {
                // Dummy
            }
        }

        return result!;
    }
}