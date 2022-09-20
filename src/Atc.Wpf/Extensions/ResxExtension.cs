namespace Atc.Wpf.Extensions;

/// <summary>
/// A markup extension to allow resources for WPF Windows and controls to be retrieved
/// from an embedded resource (resx) file associated with the window or control.
/// </summary>
[MarkupExtensionReturnType(typeof(object))]
public class ResxExtension : ManagedMarkupExtension
{
    /// <summary>
    /// Defines the handling method for the <see cref="ResxExtension.Resource" /> event.
    /// </summary>
    /// <param name="resxName">The name of the resx file.</param>
    /// <param name="key">The resource key within the file.</param>
    /// <param name="culture">The culture to get the resource for.</param>
    /// <returns>The resource.</returns>
    public delegate object ResourceHandler(string resxName, string key, CultureInfo culture);

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
    public ResxExtension(string resxName, string key)
        : base(MarkupManager)
    {
        ResxName = resxName;
        Key = key;
        DefaultValue = key;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResxExtension"/> class.
    /// </summary>
    /// <param name="resxName">The fully qualified name of the embedded resx (without .resources).</param>
    /// <param name="key">The key used to get the value from the resources.</param>
    /// <param name="defaultValue">
    /// The default value for the property (can be null).  This is useful for non-string
    /// that properties that may otherwise cause page load errors if the resource is not
    /// present for some reason (eg at design time before the assembly has been compiled).
    /// </param>
    public ResxExtension(string resxName, string key, string defaultValue)
        : base(MarkupManager)
    {
        ResxName = resxName;
        Key = key;
        DefaultValue = !string.IsNullOrEmpty(defaultValue)
            ? defaultValue
            : key;
    }

    /// <summary>
    /// Occurs when [get resource].
    /// </summary>
    [SuppressMessage("Design", "CA1003:Use generic event handler instances", Justification = "OK.")]
    [SuppressMessage("Design", "MA0046:Use EventHandler<T> to declare events", Justification = "OK.")]
    public static event ResourceHandler? Resource;

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
    public string? DefaultValue { get; }

    /// <summary>
    /// Gets or sets the suffix.</summary>
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
    public static void UpdateTarget(string key)
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
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    [SuppressMessage("Major Code Smell", "S3928:Parameter names used into ArgumentException constructors should match an existing one ", Justification = "OK.")]
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
            if (Resource is not null)
            {
                result = Resource(ResxName, Key, CultureManager.UiCulture);
            }

            if (result is null)
            {
                resourceManager ??= GlobalizationHelper.GetResourceManager(ResxName);
                result = resourceManager.GetObject(Key, CultureManager.UiCulture);
            }

            if (result is not null && !string.IsNullOrEmpty(Suffix))
            {
                result = result + Suffix;
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
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    private object GetDefaultValue(string key)
    {
        object? result = DefaultValue;
        var targetType = TargetPropertyType;
        if (DefaultValue is null)
        {
            if (targetType == typeof(string) || targetType == typeof(object))
            {
                result = "#" + key + Suffix;
            }
        }
        else
        {
            if (targetType is null || targetType == typeof(string) || targetType == typeof(object))
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