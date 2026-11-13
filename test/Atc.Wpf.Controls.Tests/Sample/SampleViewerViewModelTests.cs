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

    [Fact]
    public void CreateSampleReference_NamesTheSampleTypeTheHeaderAndTheXamlFile()
    {
        var reference = SampleViewerViewModel.CreateSampleReference(
            "AnimationLibrary",
            "Atc.Wpf.Sample.SamplesWpf.Animation.AnimationView",
            "sample/Atc.Wpf.Sample/SamplesWpf/Animation/AnimationView.xaml");

        Assert.Equal(
            "For the sample Atc.Wpf.Sample.SamplesWpf.Animation.AnimationView (\"AnimationLibrary\" in the sample app, sample/Atc.Wpf.Sample/SamplesWpf/Animation/AnimationView.xaml)",
            reference);
    }

    [Fact]
    public void CreateSampleReference_WithoutAXamlFile_LeavesThePathOut()
    {
        var reference = SampleViewerViewModel.CreateSampleReference(
            "AnimationLibrary",
            "Atc.Wpf.Sample.SamplesWpf.Animation.AnimationView",
            sampleXamlPath: null);

        Assert.Equal(
            "For the sample Atc.Wpf.Sample.SamplesWpf.Animation.AnimationView (\"AnimationLibrary\" in the sample app)",
            reference);
    }

    [StaFact]
    public void SampleItemMessage_WithoutAPath_ClearsTheSampleReference()
    {
        var sut = new SampleViewerViewModel
        {
            Header = "Badge",
        };

        Messenger.Default.Send(new SampleItemMessage(header: null, sampleItemPath: null));

        Assert.Null(sut.SampleReference);
        Assert.False(sut.CopySampleReferenceCommand.CanExecute(parameter: null));
    }
}