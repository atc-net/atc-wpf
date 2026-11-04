namespace Atc.Wpf.Forms.Dialogs;

/// <summary>
/// Owner and screen-size handling shared by the colour and font picker dialogs.
/// </summary>
internal static class PickerDialogBoxHelper
{
    /// <summary>
    /// The largest share of the work area a picker dialog takes; beyond that its content scrolls.
    /// </summary>
    internal const double MaxWorkAreaRatio = 0.95;

    /// <summary>
    /// Makes <paramref name="owningWindow"/> the owner of <paramref name="dialog"/> and centres the dialog on it, when
    /// the owner has been shown and not closed; otherwise the dialog keeps its startup location. The dialog is then
    /// limited to <see cref="MaxWorkAreaRatio"/> of the work area of the owner's monitor (the primary monitor without
    /// an owner), so it fits on small or highly scaled screens.
    /// </summary>
    public static void ApplyOwnerAndFitToWorkArea(
        Window dialog,
        Window? owningWindow)
    {
        ArgumentNullException.ThrowIfNull(dialog);

        var hasOwner = owningWindow is not null &&
                       !ReferenceEquals(owningWindow, dialog) &&
                       HasWindowHandle(owningWindow);

        var workArea = hasOwner
            ? Atc.Wpf.Controls.Layouts.DockScreenHelper.GetWorkAreaAt(
                owningWindow!,
                new Point(owningWindow!.ActualWidth / 2, owningWindow.ActualHeight / 2))
            : SystemParameters.WorkArea;

        dialog.MaxWidth = workArea.Width * MaxWorkAreaRatio;
        dialog.MaxHeight = workArea.Height * MaxWorkAreaRatio;

        if (hasOwner)
        {
            dialog.Owner = owningWindow;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.SizeChanged -= OnDialogSizeChanged;
            dialog.SizeChanged += OnDialogSizeChanged;
            dialog.ContentRendered -= OnDialogContentRendered;
            dialog.ContentRendered += OnDialogContentRendered;
        }
    }

    /// <summary>
    /// Keeps the centre of a placed dialog while its size settles before the first render, so the centring on the
    /// owner holds when the laid-out size differs from the size used to place it.
    /// </summary>
    private static void OnDialogSizeChanged(
        object sender,
        SizeChangedEventArgs e)
    {
        var dialog = (Window)sender;
        if (e.PreviousSize.Width <= 0 ||
            e.PreviousSize.Height <= 0 ||
            dialog.WindowState != WindowState.Normal ||
            double.IsNaN(dialog.Left) ||
            double.IsNaN(dialog.Top))
        {
            return;
        }

        dialog.Left -= (e.NewSize.Width - e.PreviousSize.Width) / 2;
        dialog.Top -= (e.NewSize.Height - e.PreviousSize.Height) / 2;
    }

    /// <summary>
    /// Once shown, stops following size changes (the user may resize the dialog) and moves the dialog into the work
    /// area of its monitor when centring on the owner put part of it off screen.
    /// </summary>
    private static void OnDialogContentRendered(
        object? sender,
        EventArgs e)
    {
        if (sender is not Window dialog)
        {
            return;
        }

        dialog.SizeChanged -= OnDialogSizeChanged;
        dialog.ContentRendered -= OnDialogContentRendered;

        if (dialog.WindowState != WindowState.Normal)
        {
            return;
        }

        var width = dialog.ActualWidth;
        var height = dialog.ActualHeight;
        var workArea = Atc.Wpf.Controls.Layouts.DockScreenHelper.GetWorkAreaAt(dialog, new Point(width / 2, height / 2));
        var left = System.Math.Max(workArea.Left, System.Math.Min(dialog.Left, workArea.Right - width));
        var top = System.Math.Max(workArea.Top, System.Math.Min(dialog.Top, workArea.Bottom - height));

        if (System.Math.Abs(left - dialog.Left) > 0.5)
        {
            dialog.Left = left;
        }

        if (System.Math.Abs(top - dialog.Top) > 0.5)
        {
            dialog.Top = top;
        }
    }

    /// <summary>
    /// Whether the window has been shown and not yet closed (a window without a handle cannot own another).
    /// </summary>
    private static bool HasWindowHandle(Window window)
        => new System.Windows.Interop.WindowInteropHelper(window).Handle != IntPtr.Zero &&
           PresentationSource.FromVisual(window) is not null;
}