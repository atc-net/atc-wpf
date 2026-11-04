namespace Atc.Wpf.UiTests;

/// <summary>
/// Makes the test process per-monitor DPI aware before any test runs. Without it, Windows scales the window
/// positions that UI Automation reports to a DPI-unaware process, so on mixed-DPI setups (e.g. a 200 % laptop
/// screen next to a 100 % monitor) the bounds of the main window, its dialogs and the screens come back in
/// different units and placement checks fail.
/// </summary>
internal static class DpiAwareness
{
    private static readonly IntPtr PerMonitorAwareV2 = new(-4);

    [ModuleInitializer]
    [SuppressMessage("Usage", "CA2255:The 'ModuleInitializer' attribute should not be used in libraries", Justification = "The test assembly is the process; DPI awareness must be set before the first window or automation call.")]
    internal static void Initialize()
        => _ = SetProcessDpiAwarenessContext(PerMonitorAwareV2);

    /// <summary>
    /// Gets how many physical pixels one device-independent unit takes on the monitor of the element's window
    /// (2 at 200 % scaling), so tolerances written in device-independent units can be compared with the physical
    /// pixels that UI Automation reports to this per-monitor DPI-aware process.
    /// </summary>
    /// <param name="element">An element of the window to measure (the nearest element with a window handle is used).</param>
    /// <returns>The scale; 1 when the DPI cannot be read.</returns>
    public static double GetScale(AutomationElement element)
    {
        var handle = IntPtr.Zero;
        for (var current = element; current is not null && handle == IntPtr.Zero; current = current.Parent)
        {
            handle = current.Properties.NativeWindowHandle.ValueOrDefault;
        }

        var dpi = handle == IntPtr.Zero ? 0 : GetDpiForWindow(handle);
        return dpi == 0 ? 1 : dpi / 96d;
    }

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern uint GetDpiForWindow(IntPtr hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessDpiAwarenessContext(IntPtr value);
}