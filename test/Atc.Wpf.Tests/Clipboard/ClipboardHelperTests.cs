namespace Atc.Wpf.Tests.Clipboard;

[Collection("Clipboard")]
public sealed class ClipboardHelperTests
{
    [StaFact]
    public void TrySetText_PutsTheTextOnTheClipboard()
    {
        var text = "atc-" + Guid.NewGuid().ToString("N");

        // The clipboard is shared by every process on the machine (clipboard history, RDP, a clipboard manager or
        // the user can replace it between the copy and the read), so a single overwrite is retried.
        var copied = false;
        var actual = string.Empty;
        for (var attempt = 0; attempt < 5 && actual != text; attempt++)
        {
            copied = ClipboardHelper.TrySetText(text);
            actual = ReadClipboardText();
        }

        Assert.True(copied);
        Assert.Equal(text, actual);
    }

    [StaFact]
    public void TrySetText_WithNullText_ReturnsFalse()
    {
        var copied = ClipboardHelper.TrySetText(text: null);

        Assert.False(copied);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Another process can hold the clipboard open; the caller retries.")]
    private static string ReadClipboardText()
    {
        try
        {
            return System.Windows.Clipboard.GetText();
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}