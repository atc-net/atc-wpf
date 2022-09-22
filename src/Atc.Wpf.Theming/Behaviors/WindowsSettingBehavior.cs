// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace Atc.Wpf.Theming.Behaviors;

[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
public class WindowsSettingBehavior : Behavior<NiceWindow>
{
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.SourceInitialized += OnSourceInitialized;
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        CleanUp("from OnDetaching");
        base.OnDetaching();
    }

    private void OnSourceInitialized(
        object? sender,
        EventArgs e)
    {
        LoadWindowState();

        var window = AssociatedObject;
        if (window is null)
        {
            Trace.TraceError($"{this}: Can not attach to nested events, cause the AssociatedObject is null.");
            return;
        }

        window.StateChanged += OnStateChanged;
        window.Closing += OnClosing;
        window.Closed += OnClosed;

        Application.Current?.BeginInvoke(app =>
        {
            if (app is not null)
            {
                app.SessionEnding += CurrentApplicationSessionEnding;
            }
        });
    }

    private void OnClosing(
        object? sender,
        CancelEventArgs e)
    {
        SaveWindowState();
    }

    private void OnClosed(
        object? sender,
        EventArgs e)
    {
        CleanUp("from AssociatedObject closed event");
    }

    private void CurrentApplicationSessionEnding(
        object? sender,
        SessionEndingCancelEventArgs e)
    {
        SaveWindowState();
    }

    private void OnStateChanged(
        object? sender,
        EventArgs e)
    {
        if (AssociatedObject?.WindowState == WindowState.Minimized)
        {
            SaveWindowState();
        }
    }

    private void CleanUp(
        string fromWhere)
    {
        var window = AssociatedObject;
        if (window is null)
        {
            Trace.TraceWarning($"{this}: Can not clean up {fromWhere}, cause the AssociatedObject is null. This can maybe happen if this Behavior was already detached.");
            return;
        }

        Debug.WriteLine($"{this}: Clean up {fromWhere}.");

        window.StateChanged -= OnStateChanged;
        window.Closing -= OnClosing;
        window.Closed -= OnClosed;
        window.SourceInitialized -= OnSourceInitialized;

        // This operation must be thread safe
        Application.Current?.BeginInvoke(app =>
        {
            if (app is not null)
            {
                app.SessionEnding -= CurrentApplicationSessionEnding;
            }
        });
    }

    private void LoadWindowState()
    {
        var window = AssociatedObject;
        if (window is null)
        {
            return;
        }

        var settings = window.GetWindowPlacementSettings();
        if (settings is null || !window.SaveWindowPosition)
        {
            return;
        }

        try
        {
            settings.Reload();
        }
        catch (Exception e)
        {
            Trace.TraceError($"{this}: The settings for {window} could not be reloaded! {e}");
            return;
        }

        // check for existing placement and prevent empty bounds
        if (settings.Placement is null || settings.Placement.normalPosition.IsEmpty)
        {
            return;
        }

        try
        {
            var wp = settings.Placement.ToWindowPlacement();
            WinApiHelper.SetWindowPlacement(window, wp);
        }
        catch (Exception ex)
        {
            throw new AtcAppsException($"Failed to set the window state for {window} from the settings.", ex);
        }
    }

    [SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "OK.")]
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    private void SaveWindowState()
    {
        var window = AssociatedObject;
        if (window is null)
        {
            return;
        }

        var settings = window.GetWindowPlacementSettings();
        if (settings is null || !window.SaveWindowPosition)
        {
            return;
        }

        var windowHandle = new WindowInteropHelper(window).EnsureHandle();
        var wp = new WINDOWPLACEMENT
        {
            length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>(),
        };
        unsafe
        {
            PInvoke.GetWindowPlacement((HWND)windowHandle, &wp);
        }

        // check for saveable values
        if (wp.showCmd != SHOW_WINDOW_CMD.SW_HIDE && wp.length > 0)
        {
            if (wp.showCmd == SHOW_WINDOW_CMD.SW_NORMAL)
            {
                unsafe
                {
                    var rect = new RECT
                    {
                        left = 0,
                        top = 0,
                        right = 0,
                        bottom = 0,
                    };

                    PInvoke.GetWindowRect(new HWND(windowHandle), &rect);
                    if (rect.left != 0 || rect.top != 0 || rect.right != 0 || rect.bottom != 0)
                    {
                        var monitor = PInvoke.MonitorFromWindow(new HWND(windowHandle), MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
                        if (monitor != IntPtr.Zero)
                        {
                            var monitorInfo = new MONITORINFO
                            {
                                cbSize = (uint)Marshal.SizeOf<MONITORINFO>(),
                            };

                            PInvoke.GetMonitorInfo(monitor, &monitorInfo);
                            rect.Offset(monitorInfo.rcMonitor.left - monitorInfo.rcWork.left, monitorInfo.rcMonitor.top - monitorInfo.rcWork.top);
                        }

                        wp.rcNormalPosition = rect;
                    }
                }
            }

            if (!wp.rcNormalPosition.IsEmpty())
            {
                settings.Placement = WindowPlacementSetting.FromWindowPlacement(wp);
            }
        }

        try
        {
            settings.Save();
        }
        catch (Exception e)
        {
            Trace.TraceError($"{this}: The settings could not be saved! {e}");
        }
    }
}