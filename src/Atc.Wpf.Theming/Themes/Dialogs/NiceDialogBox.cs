namespace Atc.Wpf.Theming.Themes.Dialogs;

/// <summary>
/// A <see cref="NiceWindow"/> preconfigured as a dialog: centered on screen, hidden from the taskbar,
/// without minimize, maximize/restore and close buttons, and 350 x 200 in size.
/// </summary>
public class NiceDialogBox : NiceWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NiceDialogBox"/> class.
    /// </summary>
    public NiceDialogBox()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ShowInTaskbar = false;
        ShowMinButton = false;
        ShowMaxRestoreButton = false;
        ShowCloseButton = false;
        Width = 350;
        Height = 200;
    }
}