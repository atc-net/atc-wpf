namespace Atc.Wpf.Components.Tests.Dialogs;

/// <summary>
/// Ctrl+C in a message dialog copies the text laid out like a Windows message box.
/// </summary>
public sealed class DialogBoxHelperClipboardTextTests
{
    [StaFact]
    public void CreateClipboardText_HasTitleHeaderMessageAndButtons()
    {
        var settings = new Atc.Wpf.Forms.Dialogs.DialogBoxSettings(Atc.Wpf.Forms.Dialogs.DialogBoxType.YesNoCancel)
        {
            AffirmativeButtonText = "Yes",
            NegativeButtonText = "No",
            CancelButtonText = "Cancel",
        };

        var text = DialogBoxHelper.CreateClipboardText("Close", "Recording", "Close anyway?\nIt stops 3 cameras.", settings);

        var expected = string.Join(
            Environment.NewLine,
            "---------------------------",
            "Close",
            "---------------------------",
            "Recording",
            "---------------------------",
            "Close anyway?" + Environment.NewLine + "It stops 3 cameras.",
            "---------------------------",
            "Yes   No   Cancel",
            "---------------------------");
        Assert.Equal(expected, text);
    }

    [StaFact]
    public void CreateClipboardText_WithoutHeader_LeavesItOut()
    {
        var settings = new Atc.Wpf.Forms.Dialogs.DialogBoxSettings(Atc.Wpf.Forms.Dialogs.DialogBoxType.Ok)
        {
            AffirmativeButtonText = "OK",
        };

        var text = DialogBoxHelper.CreateClipboardText("Info", header: null, "Saved.", settings);

        Assert.Equal(
            string.Join(Environment.NewLine, "---------------------------", "Info", "---------------------------", "Saved.", "---------------------------", "OK", "---------------------------"),
            text);
    }
}