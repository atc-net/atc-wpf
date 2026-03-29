// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters.StringTo;

public sealed class StringToSplitStringListValueConverterTests
{
    private readonly StringToSplitStringListValueConverter sut = new();

    [Fact]
    public void Convert_SemicolonSeparated()
    {
        // Act
        var result = sut.Convert(
            "a; b; c",
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        var list = Assert.IsType<List<string>>(result);
        Assert.Equal(3, list.Count);
        Assert.Equal("a", list[0]);
        Assert.Equal("b", list[1]);
        Assert.Equal("c", list[2]);
    }

    [Fact]
    public void Convert_SingleValue()
    {
        // Act
        var result = sut.Convert(
            "single",
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        var list = Assert.IsType<List<string>>(result);
        Assert.Single(list);
        Assert.Equal("single", list[0]);
    }

    [Fact]
    public void Convert_EmptyString()
    {
        // Act
        var result = sut.Convert(
            "",
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        var list = Assert.IsType<List<string>>(result);
        Assert.Empty(list);
    }

    [Fact]
    public void ConvertBack_List()
    {
        // Arrange
        var input = new List<string> { "a", "b" };

        // Act
        var result = sut.ConvertBack(
            input,
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.Equal("a;b", result);
    }
}