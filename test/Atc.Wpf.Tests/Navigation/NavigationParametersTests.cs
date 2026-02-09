namespace Atc.Wpf.Tests.Navigation;

public sealed class NavigationParametersTests
{
    [Fact]
    public void GetValue_ReturnsTypedValue()
    {
        var sut = new NavigationParameters
        {
            ["Id"] = 42,
        };

        var result = sut.GetValue<int>("Id");

        Assert.Equal(42, result);
    }

    [Fact]
    public void GetValue_ReturnsDefault_WhenKeyMissing()
    {
        var sut = new NavigationParameters();

        var result = sut.GetValue<int>("Missing");

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetValue_ReturnsDefault_WhenTypeMismatch()
    {
        var sut = new NavigationParameters
        {
            ["Id"] = "not-an-int",
        };

        var result = sut.GetValue<int>("Id");

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetValueOrDefault_ReturnsSpecifiedDefault()
    {
        var sut = new NavigationParameters();

        var result = sut.GetValueOrDefault("Missing", 99);

        Assert.Equal(99, result);
    }

    [Fact]
    public void GetValueOrDefault_ReturnsValue_WhenKeyExists()
    {
        var sut = new NavigationParameters
        {
            ["Count"] = 5,
        };

        var result = sut.GetValueOrDefault("Count", 99);

        Assert.Equal(5, result);
    }

    [Fact]
    public void WithParameter_AddsAndReturnsSelf()
    {
        var sut = new NavigationParameters();

        var returned = sut.WithParameter("Key", "Value");

        Assert.Same(sut, returned);
        Assert.Equal("Value", sut.GetValue<string>("Key"));
    }

    [Fact]
    public void WithParameter_FluentChaining()
    {
        var sut = new NavigationParameters()
            .WithParameter("A", 1)
            .WithParameter("B", "two")
            .WithParameter("C", true);

        Assert.Equal(1, sut.GetValue<int>("A"));
        Assert.Equal("two", sut.GetValue<string>("B"));
        Assert.True(sut.GetValue<bool>("C"));
    }
}