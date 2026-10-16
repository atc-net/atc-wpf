namespace Atc.Wpf.Components.Tests.Dialogs;

/// <summary>
/// Drives the shared owner wiring behind the dialog boxes with plain windows
/// (the real dialog boxes need theme resources that are not available in a test host).
/// </summary>
public sealed class DialogBoxHelperOwnerTests
{
    [WpfFact]
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
            Components.Dialogs.Helpers.DialogBoxHelper.ApplyOwner(dialog, owner);

            Assert.Same(owner, dialog.Owner);
            Assert.Equal(WindowStartupLocation.CenterOwner, dialog.WindowStartupLocation);
        }
        finally
        {
            dialog.Owner = null;
            owner.Close();
        }
    }

    [WpfFact]
    public void ApplyOwner_OwnerNotShown_KeepsCenterScreenAndNoOwner()
    {
        var owner = CreateHiddenWindow();
        var dialog = new Window
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
        };

        Components.Dialogs.Helpers.DialogBoxHelper.ApplyOwner(dialog, owner);

        Assert.Null(dialog.Owner);
        Assert.Equal(WindowStartupLocation.CenterScreen, dialog.WindowStartupLocation);
    }

    [WpfFact]
    public void ApplyOwner_OwnerIsNull_KeepsCenterScreenAndNoOwner()
    {
        var dialog = new Window
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
        };

        Components.Dialogs.Helpers.DialogBoxHelper.ApplyOwner(dialog, owningWindow: null);

        Assert.Null(dialog.Owner);
        Assert.Equal(WindowStartupLocation.CenterScreen, dialog.WindowStartupLocation);
    }

    [WpfFact]
    public void ApplyOwner_OwnerWasClosed_KeepsCenterScreenAndNoOwner()
    {
        var owner = CreateHiddenWindow();
        owner.Show();
        owner.Close();
        var dialog = new Window
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
        };

        Components.Dialogs.Helpers.DialogBoxHelper.ApplyOwner(dialog, owner);

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