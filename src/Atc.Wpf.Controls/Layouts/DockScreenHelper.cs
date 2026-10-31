namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Screen geometry for the floating windows of a <see cref="DockPanelPro"/>, in device-independent units.
/// </summary>
internal static class DockScreenHelper
{
    /// <summary>
    /// Moves (and if needed shrinks) window bounds so they lie inside one work area: the one they overlap most,
    /// or the nearest one when they overlap none (e.g. a monitor that is no longer attached).
    /// </summary>
    /// <param name="bounds">The window bounds.</param>
    /// <param name="workAreas">The work areas of the screens.</param>
    /// <returns>The clamped bounds; the bounds unchanged when there are no work areas.</returns>
    public static Rect ClampToWorkAreas(
        Rect bounds,
        IReadOnlyList<Rect> workAreas)
    {
        ArgumentNullException.ThrowIfNull(workAreas);

        if (bounds.IsEmpty || workAreas.Count == 0)
        {
            return bounds;
        }

        var target = GetBestWorkArea(bounds, workAreas);

        var width = System.Math.Min(bounds.Width, target.Width);
        var height = System.Math.Min(bounds.Height, target.Height);
        var left = System.Math.Min(System.Math.Max(bounds.Left, target.Left), target.Right - width);
        var top = System.Math.Min(System.Math.Max(bounds.Top, target.Top), target.Bottom - height);
        return new Rect(left, top, width, height);
    }

    /// <summary>
    /// Returns the work area that the bounds overlap most, or the nearest one when they overlap none; the primary work
    /// area when there are no work areas.
    /// </summary>
    public static Rect GetBestWorkArea(
        Rect bounds,
        IReadOnlyList<Rect> workAreas)
    {
        ArgumentNullException.ThrowIfNull(workAreas);

        if (workAreas.Count == 0)
        {
            return SystemParameters.WorkArea;
        }

        return workAreas
            .Select(area => (Area: area, Overlap: OverlapArea(bounds, area), Distance: Distance(bounds, area)))
            .OrderByDescending(x => x.Overlap)
            .ThenBy(x => x.Distance)
            .First()
            .Area;
    }

    /// <summary>
    /// Gets the work areas of all monitors in the device-independent units of the visual's window.
    /// </summary>
    public static IReadOnlyList<Rect> GetWorkAreas(Visual? reference)
    {
        var fromDevice = GetTransformFromDevice(reference);
        var areas = new List<Rect>();
        try
        {
            NativeMethods.EnumDisplayMonitors(
                IntPtr.Zero,
                IntPtr.Zero,
                (IntPtr monitor, IntPtr hdc, ref NativeMethods.NativeRect rect, IntPtr data) =>
                {
                    var info = NativeMethods.MonitorInfo.Create();
                    if (NativeMethods.GetMonitorInfo(monitor, ref info))
                    {
                        var work = info.rcWork;
                        var topLeft = fromDevice.Transform(new Point(work.Left, work.Top));
                        var bottomRight = fromDevice.Transform(new Point(work.Right, work.Bottom));
                        areas.Add(new Rect(topLeft, bottomRight));
                    }

                    return true;
                },
                IntPtr.Zero);
        }
        catch (DllNotFoundException)
        {
            // Fall back to the primary work area below.
        }
        catch (EntryPointNotFoundException)
        {
            // Fall back to the primary work area below.
        }

        if (areas.Count == 0)
        {
            areas.Add(SystemParameters.WorkArea);
        }

        return areas;
    }

    /// <summary>
    /// Converts a point of an element to screen coordinates in device-independent units.
    /// </summary>
    public static Point ToScreenDip(
        Visual element,
        Point point)
    {
        ArgumentNullException.ThrowIfNull(element);

        if (PresentationSource.FromVisual(element) is null)
        {
            return point;
        }

        var screen = element.PointToScreen(point);
        return GetTransformFromDevice(element).Transform(screen);
    }

    /// <summary>
    /// Gets the cursor position in screen coordinates in device-independent units.
    /// </summary>
    public static Point GetCursorPosition(Visual reference)
        => ToScreenDip(reference, Mouse.GetPosition((IInputElement)reference));

    private static Matrix GetTransformFromDevice(Visual? reference)
        => reference is not null && PresentationSource.FromVisual(reference)?.CompositionTarget is { } target
            ? target.TransformFromDevice
            : Matrix.Identity;

    private static double OverlapArea(
        Rect a,
        Rect b)
    {
        var overlap = Rect.Intersect(a, b);
        return overlap.IsEmpty ? 0 : overlap.Width * overlap.Height;
    }

    private static double Distance(
        Rect a,
        Rect b)
    {
        var dx = System.Math.Max(0, System.Math.Max(b.Left - a.Right, a.Left - b.Right));
        var dy = System.Math.Max(0, System.Math.Max(b.Top - a.Bottom, a.Top - b.Bottom));
        return (dx * dx) + (dy * dy);
    }

    [SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute'", Justification = "LibraryImport requires AllowUnsafeBlocks; DllImport is fine for these few user32 calls.")]
    [SuppressMessage("Naming", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Struct fields mirror the Win32 MONITORINFO layout names.")]
    private static class NativeMethods
    {
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

        [StructLayout(LayoutKind.Sequential)]
        public struct MonitorInfo
        {
            public int cbSize;
            public NativeRect rcMonitor;
            public NativeRect rcWork;
            public uint dwFlags;

            public static MonitorInfo Create()
                => new() { cbSize = Marshal.SizeOf<MonitorInfo>() };
        }

        [DllImport("user32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumDisplayMonitors(
            IntPtr hdc,
            IntPtr lprcClip,
            MonitorEnumProc lpfnEnum,
            IntPtr dwData);

        [DllImport("user32.dll", EntryPoint = "GetMonitorInfoW")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo(
            IntPtr hMonitor,
            ref MonitorInfo lpmi);
    }
}