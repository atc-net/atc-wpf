namespace Atc.Wpf.Tests.Clipboard;

[Collection("Clipboard")]
public sealed class ClipboardHelperTests
{
    [StaFact]
    public void TrySetText_PutsTheTextOnTheClipboard()
    {
        var text = "atc-" + Guid.NewGuid().ToString("N");

        var copied = ClipboardHelper.TrySetText(text);

        Assert.True(copied);
        Assert.Equal(text, System.Windows.Clipboard.GetText());
    }

    [StaFact]
    public void TrySetText_WithNullText_ReturnsFalse()
    {
        var copied = ClipboardHelper.TrySetText(text: null);

        Assert.False(copied);
    }
}