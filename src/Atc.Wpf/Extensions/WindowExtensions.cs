// ReSharper disable once CheckNamespace
namespace System.Windows;

/// <summary>
/// Extension methods for <see cref="Window"/>.
/// </summary>
public static class WindowExtensions
{
    /// <summary>
    /// Restores the window's placement from the specified placement XML.
    /// </summary>
    public static void SetPlacement(
        this Window window,
        string placementXml)
        => WindowPlacementHelper.SetPlacement(
            new WindowInteropHelper(window).Handle,
            placementXml);

    /// <summary>
    /// Gets the window's current placement serialized as XML.
    /// </summary>
    public static string GetPlacement(this Window window)
        => WindowPlacementHelper.GetPlacement(new WindowInteropHelper(window).Handle);
}