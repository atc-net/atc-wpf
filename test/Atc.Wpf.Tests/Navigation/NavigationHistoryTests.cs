namespace Atc.Wpf.Tests.Navigation;

public sealed class NavigationHistoryTests
{
    private readonly NavigationHistory sut = new();

    [Fact]
    public void Navigate_SetsCurrent()
    {
        var entry = new NavigationEntry(typeof(string));

        sut.Navigate(entry);

        Assert.Same(entry, sut.Current);
    }

    [Fact]
    public void Navigate_PushesPreviousToBackStack()
    {
        var first = new NavigationEntry(typeof(string));
        var second = new NavigationEntry(typeof(int));

        sut.Navigate(first);
        sut.Navigate(second);

        Assert.Same(second, sut.Current);
        Assert.True(sut.CanGoBack);
        Assert.Single(sut.BackStack);
    }

    [Fact]
    public void Navigate_ClearsForwardStack()
    {
        sut.Navigate(new NavigationEntry(typeof(string)));
        sut.Navigate(new NavigationEntry(typeof(int)));
        sut.GoBack();
        Assert.True(sut.CanGoForward);

        sut.Navigate(new NavigationEntry(typeof(double)));

        Assert.False(sut.CanGoForward);
        Assert.Empty(sut.ForwardStack);
    }

    [Fact]
    public void GoBack_ReturnsEntry()
    {
        var first = new NavigationEntry(typeof(string));
        sut.Navigate(first);
        sut.Navigate(new NavigationEntry(typeof(int)));

        var result = sut.GoBack();

        Assert.Same(first, result);
        Assert.Same(first, sut.Current);
    }

    [Fact]
    public void GoBack_ReturnsNull_WhenEmpty()
    {
        var result = sut.GoBack();

        Assert.Null(result);
    }

    [Fact]
    public void GoBack_PushesCurrentToForwardStack()
    {
        var first = new NavigationEntry(typeof(string));
        var second = new NavigationEntry(typeof(int));
        sut.Navigate(first);
        sut.Navigate(second);

        sut.GoBack();

        Assert.True(sut.CanGoForward);
        Assert.Single(sut.ForwardStack);
    }

    [Fact]
    public void GoForward_ReturnsEntry()
    {
        var first = new NavigationEntry(typeof(string));
        var second = new NavigationEntry(typeof(int));
        sut.Navigate(first);
        sut.Navigate(second);
        sut.GoBack();

        var result = sut.GoForward();

        Assert.Same(second, result);
        Assert.Same(second, sut.Current);
    }

    [Fact]
    public void GoForward_ReturnsNull_WhenEmpty()
    {
        var result = sut.GoForward();

        Assert.Null(result);
    }

    [Fact]
    public void Clear_ResetsAllStacks()
    {
        sut.Navigate(new NavigationEntry(typeof(string)));
        sut.Navigate(new NavigationEntry(typeof(int)));
        sut.GoBack();

        sut.Clear();

        Assert.Null(sut.Current);
        Assert.False(sut.CanGoBack);
        Assert.False(sut.CanGoForward);
        Assert.Empty(sut.BackStack);
        Assert.Empty(sut.ForwardStack);
    }
}