namespace Atc.Wpf.Hardware.Services.Internal;

[SuppressMessage("Globalization", "CA2101:Specify marshaling for P/Invoke string arguments", Justification = "Wide-char buffers used for the GetWindowText* APIs.")]
[SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute'", Justification = "LibraryImport requires AllowUnsafeBlocks; DllImport is fine for these few user32 calls.")]
internal static class NativeWindowMethods
{
    public delegate bool EnumWindowsProc(
        nint hWnd,
        nint lParam);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumWindows(
        EnumWindowsProc lpEnumFunc,
        nint lParam);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(nint hWnd);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern nint GetWindow(
        nint hWnd,
        uint uCmd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetWindowTextW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int GetWindowText(
        nint hWnd,
        [Out] char[] lpString,
        int nMaxCount);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int GetWindowTextLength(nint hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetClassNameW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int GetClassName(
        nint hWnd,
        [Out] char[] lpClassName,
        int nMaxCount);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern uint GetWindowThreadProcessId(
        nint hWnd,
        out uint lpdwProcessId);

    public static string ReadWindowTitle(nint hWnd)
    {
        var len = GetWindowTextLength(hWnd);
        if (len <= 0)
        {
            return string.Empty;
        }

        var buffer = new char[len + 1];
        var actual = GetWindowText(hWnd, buffer, buffer.Length);
        return new string(buffer, 0, actual);
    }

    public static string ReadClassName(nint hWnd)
    {
        var buffer = new char[256];
        var actual = GetClassName(hWnd, buffer, buffer.Length);
        return actual <= 0 ? string.Empty : new string(buffer, 0, actual);
    }
}