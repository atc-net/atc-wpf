namespace Atc.Wpf.Helpers;

/// <summary>
/// The GlobalizationHelper module contains procedures used to preform globalization operations.
/// </summary>
public static class GlobalizationHelper
{
    /// <summary>
    /// Cached resource managers.
    /// </summary>
    private static readonly Dictionary<string, WeakReference> ResourceManagers = new(StringComparer.Ordinal);

    /// <summary>
    /// Get the resource manager for this type.
    /// </summary>
    /// <param name="resxName">The name of the embedded resx.</param>
    /// <returns>The resource manager.</returns>
    /// <remarks>Caches resource managers to improve performance.</remarks>
    public static ResourceManager GetResourceManager(string resxName)
    {
        ArgumentNullException.ThrowIfNull(resxName);

        // ReSharper disable once InlineOutVariableDeclaration
        WeakReference? reference;
        ResourceManager? resourceManager = null;
        if (ResourceManagers.TryGetValue(resxName, out reference))
        {
            resourceManager = reference.Target as ResourceManager;

            // If the resource manager has been garbage collected then remove the cache
            // entry (it will be re-added)
            if (resourceManager is null)
            {
                ResourceManagers.Remove(resxName);
            }
        }

        if (resourceManager is null)
        {
            var assembly = AssemblyHelper.FindResourceAssembly(resxName);
            if (assembly is not null)
            {
                resourceManager = new ResourceManager(resxName, assembly);
                ResourceManagers.Add(resxName, new WeakReference(resourceManager));
            }
        }

        if (resourceManager is null)
        {
            // ReSharper disable once LocalizableElement
            throw new ArgumentOutOfRangeException(nameof(resxName), $"Can't find resource '{resxName}' in any assemblies.");
        }

        return resourceManager;
    }

    /// <summary>
    /// Gets the object from resource.
    /// </summary>
    /// <param name="resxName">Name of the RESX.</param>
    /// <param name="key">The key.</param>
    public static object? GetObjectFromResource(string resxName, string key)
    {
        return GetResourceManager(resxName).GetObject(key, CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Gets the stream from resource.
    /// </summary>
    /// <param name="resxName">Name of the RESX.</param>
    /// <param name="key">The key.</param>
    public static Stream? GetStreamFromResource(string resxName, string key)
    {
        return GetResourceManager(resxName).GetStream(key, CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Gets the string from resource.
    /// </summary>
    /// <param name="resxName">Name of the RESX.</param>
    /// <param name="key">The key.</param>
    public static string? GetStringFromResource(string resxName, string key)
    {
        return GetResourceManager(resxName).GetString(key, CultureInfo.CurrentUICulture);
    }
}