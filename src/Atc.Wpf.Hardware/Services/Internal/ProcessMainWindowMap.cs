namespace Atc.Wpf.Hardware.Services.Internal;

/// <summary>
/// Finds the main window of every process in one pass over the top-level windows.
/// </summary>
/// <remarks>
/// Uses the same rule as <see cref="System.Diagnostics.Process.MainWindowHandle"/>: the first top-level window,
/// in <c>EnumWindows</c> (z-) order, that belongs to the process, has no owner and is visible. Reading
/// <c>MainWindowHandle</c> per process enumerates all top-level windows once for every process.
/// </remarks>
internal static class ProcessMainWindowMap
{
    private const uint GW_OWNER = 4;

    /// <summary>
    /// Enumerates the top-level windows and maps each process id to its main window.
    /// </summary>
    /// <returns>The main window handle per process id; processes without a main window are not included.</returns>
    public static Dictionary<int, IntPtr> Capture()
    {
        var windows = new List<IntPtr>();

        NativeWindowMethods.EnumWindows(
            (hWnd, _) =>
            {
                windows.Add(hWnd);
                return true;
            },
            lParam: IntPtr.Zero);

        return Build(windows, ReadProcessId, IsMainWindow);
    }

    /// <summary>
    /// Maps each process id to the first of its windows that <paramref name="isMainWindow"/> accepts.
    /// </summary>
    /// <param name="windowsInZOrder">The top-level windows in <c>EnumWindows</c> order.</param>
    /// <param name="getProcessId">Returns the id of the process that owns a window.</param>
    /// <param name="isMainWindow">Returns whether a window qualifies as a main window.</param>
    /// <returns>The main window handle per process id.</returns>
    internal static Dictionary<int, IntPtr> Build(
        IEnumerable<IntPtr> windowsInZOrder,
        Func<IntPtr, int> getProcessId,
        Func<IntPtr, bool> isMainWindow)
    {
        var map = new Dictionary<int, IntPtr>();

        foreach (var hWnd in windowsInZOrder)
        {
            var processId = getProcessId(hWnd);
            if (!map.ContainsKey(processId) &&
                isMainWindow(hWnd))
            {
                map[processId] = hWnd;
            }
        }

        return map;
    }

    private static int ReadProcessId(IntPtr hWnd)
    {
        // A window closed in the meantime reports process id 0, and is then not visible either.
        _ = NativeWindowMethods.GetWindowThreadProcessId(hWnd, out var processId);
        return (int)processId;
    }

    private static bool IsMainWindow(IntPtr hWnd)
        => NativeWindowMethods.GetWindow(hWnd, GW_OWNER) == IntPtr.Zero &&
           NativeWindowMethods.IsWindowVisible(hWnd);
}