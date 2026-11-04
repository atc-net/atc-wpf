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

    [DllImport("user32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessDpiAwarenessContext(IntPtr value);
}