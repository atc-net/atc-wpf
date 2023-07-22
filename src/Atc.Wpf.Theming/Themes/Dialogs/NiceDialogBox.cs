namespace Atc.Wpf.Theming.Themes.Dialogs;

public class NiceDialogBox : NiceWindow
{
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