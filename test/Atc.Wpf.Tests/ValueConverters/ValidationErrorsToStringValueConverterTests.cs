namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ValidationErrorsToStringValueConverterTests
{
    private readonly IValueConverter converter = new ValidationErrorsToStringValueConverter();

    private readonly KeyValuePair<string, string>[] inputSet1 =
    {
        new("Key1", "Error1"),
    };

    private readonly KeyValuePair<string, string>[] inputSet2 =
    {
        new("Key1", "Error1"),
        new("Key2", "Error2"),
    };

    [Theory]
    [InlineData("Error1", 1)]
    [InlineData("Error1\nError2", 2)]
    public void Convert(string expected, int inputSetNumber)
    {
        // Arrange
        var errors = new ObservableCollection<ValidationError>();
        var testData = inputSetNumber switch
        {
            1 => inputSet1,
            2 => inputSet2,
            _ => Array.Empty<KeyValuePair<string, string>>(),
        };

        foreach (var (key, value) in testData)
        {
            errors.Add(new ValidationError(new DataErrorValidationRule(), key, value, exception: null));
        }

        var input = new ReadOnlyObservableCollection<ValidationError>(errors);

        // Atc
        var actual = converter.Convert(input, targetType: null, parameter: null, culture: null);

        // Arrange
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ConvertBack_Should_Throw_Exception()
    {
        // Act
        var exception = Record.Exception(() => converter.ConvertBack(value: null, targetType: null, parameter: null, culture: null));

        // Assert
        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}