// ReSharper disable once CheckNamespace
namespace System.Windows.Markup;

/// <summary>
/// Defines a class for managing <see cref="ManagedMarkupExtension" /> objects.
/// </summary>
/// <remarks>
/// This class provides a single point for updating all markup targets that use the given Markup
/// Extension managed by this class.
/// </remarks>
public sealed class MarkupExtensionManager
{
    /// <summary>
    /// The interval at which to cleanup and remove extensions.
    /// </summary>
    private readonly int cleanupInterval;

    /// <summary>
    /// The number of extensions added since the last cleanup.
    /// </summary>
    private int cleanupCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkupExtensionManager" /> class.
    /// </summary>
    /// <param name="cleanupInterval">
    /// The interval at which to cleanup and remove extensions associated with garbage
    /// collected targets.  This specifies the number of new Markup Extensions that are
    /// created before a cleanup is triggered.
    /// </param>
    public MarkupExtensionManager(int cleanupInterval)
    {
        this.cleanupInterval = cleanupInterval;
    }

    /// <summary>
    /// Gets the active extensions.
    /// </summary>
    public IList<ManagedMarkupExtension> ActiveExtensions { get; private set; } = new List<ManagedMarkupExtension>();

    /// <summary>
    /// Force the update of all active targets that use the markup extension.
    /// </summary>
    public void UpdateAllTargets()
    {
        foreach (ManagedMarkupExtension extension in ActiveExtensions)
        {
            extension.UpdateTarget();
        }
    }

    /// <summary>
    /// Cleanup references to extensions for targets which have been garbage collected.
    /// </summary>
    /// <remarks>
    /// This method is called periodically as new <see cref="ManagedMarkupExtension" /> objects
    /// are registered to release <see cref="ManagedMarkupExtension" /> objects which are no longer
    /// required (because their target has been garbage collected).  This method does
    /// not need to be called externally, however it can be useful to call it prior to calling
    /// GC.Collect to verify that objects are being garbage collected correctly.
    /// </remarks>
    public void CleanupInactiveExtensions()
    {
        var newExtensions = new List<ManagedMarkupExtension>(ActiveExtensions.Count);
        newExtensions.AddRange(ActiveExtensions.Where(ext => ext.IsTargetAlive));
        ActiveExtensions = newExtensions;
    }

    /// <summary>
    /// Register a new extension and remove extensions which reference target objects
    /// that have been garbage collected.
    /// </summary>
    /// <param name="extension">The extension to register.</param>
    internal void RegisterExtension(ManagedMarkupExtension extension)
    {
        if (extension is null)
        {
            throw new ArgumentNullException(nameof(extension));
        }

        // Cleanup extensions for target objects which have been garbage collected
        // for performance only do this periodically
        if (cleanupCount > cleanupInterval)
        {
            CleanupInactiveExtensions();
            cleanupCount = 0;
        }

        ActiveExtensions.Add(extension);
        cleanupCount++;
    }
}