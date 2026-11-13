namespace Atc.Wpf.Controls.Tests.Sample;

// Selecting a sample used to scan the whole solution tree for its .xaml file; the index scans once per folder.
public sealed class SampleFileIndexTests
{
    [Fact]
    public void FindFolder_returns_the_folder_of_the_only_matching_view()
    {
        var sut = new SampleFileIndex([@"C:\repo\sample\App\SamplesWpf\Animation\AnimationView.xaml"]);

        var folder = sut.FindFolder("AnimationView", "App.SamplesWpf.Animation.AnimationView");

        Assert.Equal(@"C:\repo\sample\App\SamplesWpf\Animation", folder?.FullName);
    }

    [Fact]
    public void FindFolder_picks_the_view_whose_path_matches_the_namespace()
    {
        var sut = new SampleFileIndex(
        [
            @"C:\repo\sample\App\SamplesWpfControls\Buttons\ButtonView.xaml",
            @"C:\repo\sample\App\SamplesWpfTheming\Buttons\ButtonView.xaml",
        ]);

        var folder = sut.FindFolder("ButtonView", "App.SamplesWpfTheming.Buttons.ButtonView");

        Assert.Equal(@"C:\repo\sample\App\SamplesWpfTheming\Buttons", folder?.FullName);
    }

    [Fact]
    public void FindFolder_without_a_match_returns_null()
    {
        var sut = new SampleFileIndex([@"C:\repo\sample\App\SamplesWpf\Animation\AnimationView.xaml"]);

        Assert.Null(sut.FindFolder("MissingView", "App.MissingView"));
    }

    [Fact]
    public void For_scans_a_folder_once_and_skips_build_output()
    {
        var root = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "atc-sample-index-" + Guid.NewGuid().ToString("N")));
        try
        {
            Directory.CreateDirectory(Path.Combine(root.FullName, "Views"));
            Directory.CreateDirectory(Path.Combine(root.FullName, "obj"));
            File.WriteAllText(Path.Combine(root.FullName, "Views", "FirstView.xaml"), "<x />");
            File.WriteAllText(Path.Combine(root.FullName, "obj", "OutputView.xaml"), "<x />");

            var first = SampleFileIndex.For(root.FullName);
            File.WriteAllText(Path.Combine(root.FullName, "Views", "LaterView.xaml"), "<x />");
            var second = SampleFileIndex.For(root.FullName);

            Assert.Same(first, second);
            Assert.NotNull(second.FindFolder("FirstView", "X.Views.FirstView"));
            Assert.Null(second.FindFolder("OutputView", "X.obj.OutputView"));
            Assert.Null(second.FindFolder("LaterView", "X.Views.LaterView"));
        }
        finally
        {
            root.Delete(recursive: true);
        }
    }
}