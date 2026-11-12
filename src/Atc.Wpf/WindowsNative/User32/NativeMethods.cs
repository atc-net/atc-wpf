// ReSharper disable IdentifierTypo
namespace Atc.Wpf.WindowsNative.User32;

internal static class NativeMethods
{
    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    internal static extern bool SetWindowPlacement(
        nint hWnd,
        [In] ref WINDOWPLACEMENT lpwndpl);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    internal static extern bool GetWindowPlacement(
        nint hWnd,
        out WINDOWPLACEMENT lpwndpl);

    [DllImport("user32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool AddClipboardFormatListener(nint hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RemoveClipboardFormatListener(nint hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RegisterHotKey(
        nint hWnd,
        int id,
        uint fsModifiers,
        uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnregisterHotKey(
        nint hWnd,
        int id);
}