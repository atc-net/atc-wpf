namespace Atc.Wpf.Forms.Tests.Dialogs;

/// <summary>
/// The colour and font picker dialogs are owned by (and centred on) a shown owner, and never get larger than the
/// work area, so their buttons stay on screen on small or highly scaled monitors.
/// </summary>
public sealed class PickerDialogBoxHelperTests
{
    [StaFact]
    public void OwnerIsShown_OwnsAndCentresTheDialog_AndLimitsItToTheWorkArea()
    {
        var owner = CreateHiddenWindow();
        owner.Show();
        var dialog = new Window { Width = 5000, Height = 5000 };

        try
        {
            Atc.Wpf.Forms.Dialogs.PickerDialogBoxHelper.ApplyOwnerAndFitToWorkArea(dialog, owner);

            Assert.Same(owner, dialog.Owner);
            Assert.Equal(WindowStartupLocation.CenterOwner, dialog.WindowStartupLocation);
            Assert.True(dialog.MaxWidth > 0 && double.IsFinite(dialog.MaxWidth));
            Assert.True(dialog.MaxHeight > 0 && double.IsFinite(dialog.MaxHeight));
        }
        finally
        {
            dialog.Owner = null;
            owner.Close();
        }
    }

    [StaFact]
    public void NoOwner_KeepsTheStartupLocation_AndUsesThePrimaryWorkArea()
    {
        var dialog = new Window { WindowStartupLocation = WindowStartupLocation.CenterScreen };

        Atc.Wpf.Forms.Dialogs.PickerDialogBoxHelper.ApplyOwnerAndFitToWorkArea(dialog, owningWindow: null);

        Assert.Null(dialog.Owner);
        Assert.Equal(WindowStartupLocation.CenterScreen, dialog.WindowStartupLocation);
        Assert.Equal(SystemParameters.WorkArea.Width * Atc.Wpf.Forms.Dialogs.PickerDialogBoxHelper.MaxWorkAreaRatio, dialog.MaxWidth, precision: 1);
        Assert.Equal(SystemParameters.WorkArea.Height * Atc.Wpf.Forms.Dialogs.PickerDialogBoxHelper.MaxWorkAreaRatio, dialog.MaxHeight, precision: 1);
    }

    [StaFact]
    public void OwnerNotShown_KeepsTheStartupLocationAndNoOwner()
    {
        var owner = CreateHiddenWindow();
        var dialog = new Window { WindowStartupLocation = WindowStartupLocation.CenterScreen };

        Atc.Wpf.Forms.Dialogs.PickerDialogBoxHelper.ApplyOwnerAndFitToWorkArea(dialog, owner);

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