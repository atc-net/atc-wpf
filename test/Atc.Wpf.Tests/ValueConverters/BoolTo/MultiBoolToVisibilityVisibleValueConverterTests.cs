// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class MultiBoolToVisibilityVisibleValueConverterTests
{
    private readonly IMultiValueConverter converter = new MultiBoolToVisibilityVisibleValueConverter();

    private static readonly object[] AllTrue = [true, true, true];
    private static readonly object[] OneFalse = [true, false, true];
    private static readonly object[] AllFalse = [false, false, false];
    private static readonly object[] Empty = [];

    [Theory]
    [InlineData(true, nameof(AllTrue))]
    [InlineData(false, nameof(OneFalse))]
    [InlineData(true, nameof(Empty))]
    public void Convert_AND_DefaultOperator(bool expected, string inputSetName)
    {
        // Arrange
        var input = GetInput(inputSetName);

        // Act
        var actual = converter.Convert(input, targetType: null!, parameter: null, culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true, nameof(AllTrue))]
    [InlineData(true, nameof(OneFalse))]
    [InlineData(false, nameof(AllFalse))]
    [InlineData(false, nameof(Empty))]
    public void Convert_OR_WithEnumParameter(bool expected, string inputSetName)
    {
        // Arrange
        var input = GetInput(inputSetName);

        // Act
        var actual = converter.Convert(
            input,
            targetType: null!,
            parameter: BooleanOperatorType.OR,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Convert_OR_WithStringParameter()
    {
        // Act
        var actual = converter.Convert(
            OneFalse,
            targetType: null!,
            parameter: "or",
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.True((bool)actual!);
    }

    private static object[] GetInput(string name) => name switch
    {
        nameof(AllTrue) => AllTrue,
        nameof(OneFalse) => OneFalse,
        nameof(AllFalse) => AllFalse,
        nameof(Empty) => Empty,
        _ => throw new ArgumentOutOfRangeException(nameof(name)),
    };
}