// ReSharper disable once CheckNamespace
namespace System.Windows;

public static class WindowExtensions
{
    public static void SetPlacement(this Window window, string placementXml)
    {
        WindowPlacementHelper.SetPlacement(new WindowInteropHelper(window).Handle, placementXml);
    }

    public static string GetPlacement(this Window window)
    {
        return WindowPlacementHelper.GetPlacement(new WindowInteropHelper(window).Handle);
    }
}