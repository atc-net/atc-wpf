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

    // A UserControl's ToString() adds the text of its content; with a dot in it the old lookup took a fragment of that text.
    [Fact]
    public void GetSampleClassName_UsesTheTypeName_NotToString()
        => Assert.Equal(
            nameof(DottedTextSampleView),
            SampleViewerViewModel.GetSampleClassName(typeof(DottedTextSampleView)));

    [StaFact]
    public void GetViewModelClassName_WithoutDataContextOrWithTheViewerViewModel_IsNull()
    {
        Assert.Null(SampleViewerViewModel.GetViewModelClassName(dataContext: null));
        Assert.Null(SampleViewerViewModel.GetViewModelClassName(new SampleViewerViewModel()));
    }

    [Fact]
    public void GetViewModelClassName_UsesTheTypeName_NotToString()
        => Assert.Equal(
            nameof(DottedTextViewModel),
            SampleViewerViewModel.GetViewModelClassName(new DottedTextViewModel()));

    private sealed class DottedTextSampleView : UserControl
    {
        public override string ToString()
            => "X.Y.DotSampleView Content:v1.2";
    }

    private sealed class DottedTextViewModel
    {
        public override string ToString()
            => "a.b Content:1.5";
    }
}