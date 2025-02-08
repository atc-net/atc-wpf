// ReSharper disable MergeIntoPattern
namespace Atc.Wpf.Theming.Behaviors.Internal;

internal static class WinApiHelper
{
    private static SafeHandle? user32;

    /// <summary>
    /// Get caption for the given id from the user32.dll.
    /// </summary>
    /// <param name="id">The id for the caption.</param>
    /// <returns>The caption from the id.</returns>
    public static unsafe string GetCaption(uint id)
    {
        user32 ??= PInvoke.LoadLibrary(System.IO.Path.Combine(Environment.SystemDirectory, "User32.dll"));

        Span<char> buffer = new char[256];

        var result = PInvoke.LoadString(user32, id, buffer, buffer.Length);
        return result == 0 ? $"String with id '{id}' could not be found." :
            new string(buffer.Slice(0, result)).Replace("&", string.Empty, StringComparison.Ordinal);
    }

    /// <summary>
    /// Get the working area size of the monitor from where the visual stays.
    /// </summary>
    /// <param name="visual">The visual element to get the monitor information.</param>
    /// <returns>The working area size of the monitor.</returns>
    public static unsafe Size GetMonitorWorkSize(this Visual? visual)
    {
        if (visual is null
            || PresentationSource.FromVisual(visual) is not HwndSource source
            || source.IsDisposed
            || source.RootVisual is null
            || source.Handle == IntPtr.Zero)
        {
            return default;
        }

        // Try to get the monitor from where the owner stays and use the working area for window size properties
        var monitor = PInvoke.MonitorFromWindow(new HWND(source.Handle), MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
        if (monitor == IntPtr.Zero)
        {
            return default;
        }

        var monitorInfo = new MONITORINFO
        {
            cbSize = (uint)Marshal.SizeOf<MONITORINFO>(),
        };

        PInvoke.GetMonitorInfo(monitor, &monitorInfo);
        return new Size(monitorInfo.rcWork.right - monitorInfo.rcWork.left, monitorInfo.rcWork.bottom - monitorInfo.rcWork.top);
    }

    /// <summary>
    /// Sets a window placement to a window.
    /// </summary>
    /// <param name="window">The window which should get the window placement.</param>
    /// <param name="wp">The window placement for the window.</param>
    public static unsafe void SetWindowPlacement(Window? window, WINDOWPLACEMENT? wp)
    {
        if (window is null)
        {
            return;
        }

        var x = CalcIntValue(wp?.rcNormalPosition.left, window.Left);
        var y = CalcIntValue(wp?.rcNormalPosition.top, window.Top);
        var width = CalcIntValue(wp?.rcNormalPosition.GetWidth(), window.ActualWidth);
        var height = CalcIntValue(wp?.rcNormalPosition.GetHeight(), window.ActualHeight);

        var placement = new WINDOWPLACEMENT
        {
            showCmd = wp is null || wp.Value.showCmd == SHOW_WINDOW_CMD.SW_SHOWMINIMIZED ? SHOW_WINDOW_CMD.SW_SHOWNORMAL : wp.Value.showCmd,
            rcNormalPosition = new RECT
            {
                left = x,
                top = y,
                right = x + width,
                bottom = y + height,
            },
        };

        var hWnd = new WindowInteropHelper(window).EnsureHandle();
        if (!PInvoke.SetWindowPlacement(new HWND(hWnd), &placement))
        {
            Trace.TraceWarning($"{window}: The window placement {wp} could not be (SetWindowPlacement)!");
        }
    }

    private static int CalcIntValue(double? value, double fallback)
    {
        if (value is null)
        {
            return (int)fallback;
        }

        var d = (double)value;
        if (!double.IsNaN(d) && d > int.MinValue && d < int.MaxValue)
        {
            return (int)d;
        }

        return (int)fallback;
    }

    /// <summary> Gets the text associated with the given window handle. </summary>
    /// <param name="window"> The window to act on. </param>
    /// <returns> The window text. </returns>
    internal static string? GetWindowText(this Window? window)
    {
        if (window is null ||
            PresentationSource.FromVisual(window) is not HwndSource source ||
            source.IsDisposed ||
            source.RootVisual is null ||
            source.Handle == IntPtr.Zero)
        {
            return null;
        }

        var bufferSize = PInvoke.GetWindowTextLength(new HWND(source.Handle)) + 1;
        unsafe
        {
            fixed (char* windowNameChars = new char[bufferSize])
            {
                return PInvoke.GetWindowText(new HWND(source.Handle), windowNameChars, bufferSize) > 0
                    ? new string(windowNameChars)
                    : null;
            }
        }
    }

    /// <summary> Gets the text associated with the given window handle. </summary>
    /// <param name="window"> The window to act on. </param>
    /// <returns> The window text. </returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    internal static Rect GetWindowBoundingRectangle(this Window? window)
    {
        var bounds = new Rect(0, 0, 0, 0);

        if (window is null
            || PresentationSource.FromVisual(window) is not HwndSource source
            || source.IsDisposed
            || source.RootVisual is null
            || source.Handle == IntPtr.Zero)
        {
            return bounds;
        }

        var rc = new RECT
        {
            left = 0,
            top = 0,
            right = 0,
            bottom = 0,
        };

        try
        {
            unsafe
            {
                PInvoke.GetWindowRect((HWND)source.Handle, &rc);
            }
        }
        catch (Win32Exception)
        {
            // Skip
        }

        bounds = new Rect(rc.left, rc.top, rc.GetWidth(), rc.GetHeight());

        return bounds;
    }
}