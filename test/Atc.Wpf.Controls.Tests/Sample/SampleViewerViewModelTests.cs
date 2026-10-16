namespace Atc.Wpf.Controls.Tests.Sample;

public sealed class SampleViewerViewModelTests
{
    // A category node sends a message without a path: everything from the previous sample must go, including the readme.
    [StaFact]
    public void SampleItemMessage_WithoutAPath_ClearsTheReadmeAndSelectsTheFirstTab()
    {
        var sut = new SampleViewerViewModel
        {
            Header = "Badge",
            XamlCode = "<Badge />",
            MarkdownDocument = "# Badge",
            StartOnMarkdownDocument = true,
            TabSelectedIndex = 4,
        };

        Messenger.Default.Send(new SampleItemMessage(header: null, sampleItemPath: null));

        Assert.Null(sut.Header);
        Assert.Null(sut.XamlCode);
        Assert.Null(sut.MarkdownDocument);
        Assert.False(sut.HasMarkdownDocument);
        Assert.False(sut.StartOnMarkdownDocument);
        Assert.Equal(0, sut.TabSelectedIndex);
    }
}