namespace Atc.Wpf.Forms.Tests.Dialogs;

/// <summary>
/// Drives the shared owner wiring behind the dialog boxes with plain windows
/// (the real dialog boxes need theme resources that are not available in a test host).
/// </summary>
public sealed class DialogBoxWindowHelperOwnerTests
{
    [StaFact]
    public void ApplyOwner_OwnerIsShown_SetsOwnerAndCentersOnIt()
    {
        var owner = CreateHiddenWindow();
        owner.Show();
        var dialog = new Window
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
        };

        try
        {
            Atc.Wpf.Forms.Dialogs.DialogBoxWindowHelper.ApplyOwner(dialog, owner);

            Assert.Same(owner, dialog.Owner);
            Assert.Equal(WindowStartupLocation.CenterOwner, dialog.WindowStartupLocation);
        }
        finally
        {
            dialog.Owner = null;
            owner.Close();
        }
    }

    [StaFact]
    public void ApplyOwner_OwnerNotShown_KeepsCenterScreenAndNoOwner()
    {
        var owner = CreateHiddenWindow();
        var dialog = new Window
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
        };

        Atc.Wpf.Forms.Dialogs.DialogBoxWindowHelper.ApplyOwner(dialog, owner);

        Assert.Null(dialog.Owner);
        Assert.Equal(WindowStartupLocation.CenterScreen, dialog.WindowStartupLocation);
    }

    [StaFact]
    public void ApplyOwner_OwnerIsNull_KeepsCenterScreenAndNoOwner()
    {
        var dialog = new Window
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
        };

        Atc.Wpf.Forms.Dialogs.DialogBoxWindowHelper.ApplyOwner(dialog, owningWindow: null);

        Assert.Null(dialog.Owner);
        Assert.Equal(WindowStartupLocation.CenterScreen, dialog.WindowStartupLocation);
    }

    [StaFact]
    public void ApplyOwner_OwnerWasClosed_KeepsCenterScreenAndNoOwner()
    {
        var owner = CreateHiddenWindow();
        owner.Show();
        owner.Close();
        var dialog = new Window
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
        };

        Atc.Wpf.Forms.Dialogs.DialogBoxWindowHelper.ApplyOwner(dialog, owner);

        Assert.Null(dialog.Owner);
        Assert.Equal(WindowStartupLocation.CenterScreen, dialog.WindowStartupLocation);
    }

    private static Window CreateHiddenWindow()
        => new()
        {
            Width = 100,
            Height = 100,
            Left = -10000,
            Top = -10000,
            WindowStartupLocation = WindowStartupLocation.Manual,
            WindowStyle = WindowStyle.None,
            ShowInTaskbar = false,
            ShowActivated = false,
        };
}