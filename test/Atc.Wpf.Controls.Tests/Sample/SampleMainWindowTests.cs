namespace Atc.Wpf.Controls.Tests.Sample;

[Collection(nameof(PackResourceTestGroup))]
public sealed class SampleMainWindowTests
{
    [StaFact]
    public void Content_IsTheExplorer()
    {
        var sut = new SampleMainWindow();

        Assert.Same(sut.Explorer, sut.Content);
    }

    [StaFact]
    public void Categories_ForwardToTheExplorer()
    {
        var sut = new SampleMainWindow();

        sut.Categories.Add(new SampleCategory { Key = "Wpf", Header = "Wpf", TreeType = typeof(TreeView) });

        Assert.Single(sut.Explorer.Categories);
    }

    [StaFact]
    public void StatusText_ForwardsToTheExplorer()
    {
        var sut = new SampleMainWindow { StatusText = "Offline sample data" };

        Assert.Equal("Offline sample data", sut.Explorer.StatusText);
    }

    [StaFact]
    public void Defaults_SizeAndViewModel()
    {
        var sut = new SampleMainWindow();

        Assert.Equal(1600, sut.Width);
        Assert.Equal(1000, sut.Height);
        Assert.Equal(1024, sut.MinWidth);
        Assert.Equal(768, sut.MinHeight);
        Assert.Equal(WindowStartupLocation.CenterScreen, sut.WindowStartupLocation);
        Assert.IsAssignableFrom<IMainWindowViewModelBase>(sut.DataContext);
    }

    [StaFact]
    public void WindowState_IsBoundOnlyToAMainWindowViewModel()
    {
        var sut = new SampleMainWindow();
        Assert.NotNull(BindingOperations.GetBinding(sut, Window.WindowStateProperty));

        sut.DataContext = new object();

        Assert.Null(BindingOperations.GetBinding(sut, Window.WindowStateProperty));
    }
}