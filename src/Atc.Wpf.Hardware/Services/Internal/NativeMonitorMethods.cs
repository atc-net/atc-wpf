namespace Atc.Wpf.Hardware.Services.Internal;

[SuppressMessage("Globalization", "CA2101:Specify marshaling for P/Invoke string arguments", Justification = "MONITORINFOEX uses a fixed-length wide-char buffer.")]
[SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute'", Justification = "LibraryImport requires AllowUnsafeBlocks; DllImport is fine for these few user32 calls.")]
[SuppressMessage("Naming", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Struct fields mirror the Win32 MONITORINFOEX layout names.")]
internal static class NativeMonitorMethods
{
    public const uint MONITORINFOF_PRIMARY = 1;

    public delegate bool MonitorEnumProc(
        IntPtr hMonitor,
        IntPtr hdcMonitor,
        ref NativeRect lprcMonitor,
        IntPtr dwData);

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct MonitorInfoEx
    {
        public int cbSize;
        public NativeRect rcMonitor;
        public NativeRect rcWork;
        public uint dwFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szDevice;

        public static MonitorInfoEx Default()
            => new()
            {
                cbSize = Marshal.SizeOf<MonitorInfoEx>(),
                szDevice = string.Empty,
            };
    }

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumDisplayMonitors(
        IntPtr hdc,
        IntPtr lprcClip,
        MonitorEnumProc lpfnEnum,
        IntPtr dwData);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetMonitorInfoW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetMonitorInfo(
        IntPtr hMonitor,
        ref MonitorInfoEx lpmi);
}