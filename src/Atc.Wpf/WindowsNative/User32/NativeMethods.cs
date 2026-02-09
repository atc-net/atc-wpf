// ReSharper disable IdentifierTypo
namespace Atc.Wpf.WindowsNative.User32;

internal static class NativeMethods
{
    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    internal static extern bool SetWindowPlacement(
        IntPtr hWnd,
        [In] ref WINDOWPLACEMENT lpwndpl);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    internal static extern bool GetWindowPlacement(
        IntPtr hWnd,
        out WINDOWPLACEMENT lpwndpl);

    [DllImport("user32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool AddClipboardFormatListener(IntPtr hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RegisterHotKey(
        IntPtr hWnd,
        int id,
        uint fsModifiers,
        uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnregisterHotKey(
        IntPtr hWnd,
        int id);
}