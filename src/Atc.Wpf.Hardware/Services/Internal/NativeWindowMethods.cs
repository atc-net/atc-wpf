namespace Atc.Wpf.Hardware.Services.Internal;

[SuppressMessage("Globalization", "CA2101:Specify marshaling for P/Invoke string arguments", Justification = "Wide-char buffers used for the GetWindowText* APIs.")]
[SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute'", Justification = "LibraryImport requires AllowUnsafeBlocks; DllImport is fine for these few user32 calls.")]
internal static class NativeWindowMethods
{
    public delegate bool EnumWindowsProc(
        IntPtr hWnd,
        IntPtr lParam);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumWindows(
        EnumWindowsProc lpEnumFunc,
        IntPtr lParam);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetWindowTextW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int GetWindowText(
        IntPtr hWnd,
        [Out] char[] lpString,
        int nMaxCount);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetClassNameW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int GetClassName(
        IntPtr hWnd,
        [Out] char[] lpClassName,
        int nMaxCount);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern uint GetWindowThreadProcessId(
        IntPtr hWnd,
        out uint lpdwProcessId);

    public static string ReadWindowTitle(IntPtr hWnd)
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

    public static string ReadClassName(IntPtr hWnd)
    {
        var buffer = new char[256];
        var actual = GetClassName(hWnd, buffer, buffer.Length);
        return actual <= 0 ? string.Empty : new string(buffer, 0, actual);
    }
}