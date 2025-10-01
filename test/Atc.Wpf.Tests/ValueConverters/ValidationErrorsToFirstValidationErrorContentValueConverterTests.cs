namespace Atc.Wpf.Tests.ValueConverters;

public class ValidationErrorsToFirstValidationErrorContentValueConverterTests
{
    private readonly IValueConverter converter = new ValidationErrorsToFirstValidationErrorContentValueConverter();

    [Fact]
    public void Convert_ReturnsNull_WhenValueIsNull()
    {
        var result = converter.Convert(
            value: null,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.Null(result);
    }

    [Fact]
    public void Convert_ReturnsNull_WhenValueIsWrongType()
    {
        var result = converter.Convert(
            value: new object(),
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.Null(result);
    }

    [Fact]
    public void Convert_ReturnsNull_WhenCollectionIsEmpty()
    {
        var errors = new ObservableCollection<ValidationError>();
        var ro = new ReadOnlyObservableCollection<ValidationError>(errors);

        var result = converter.Convert(
            value: ro,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.Null(result);
    }

    [Fact]
    public void Convert_ReturnsFirstErrorContent_WhenCollectionHasItems()
    {
        var errors = new ObservableCollection<ValidationError>
        {
            new(new ExceptionValidationRule(), new object(), "First error", null),
            new(new ExceptionValidationRule(), new object(), "Second error", null),
        };

        var ro = new ReadOnlyObservableCollection<ValidationError>(errors);

        var result = converter.Convert(
            value: ro,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.Equal("First error", result);
    }

    [Fact]
    public void Convert_ReturnsNull_WhenFirstErrorContentIsNull()
    {
        var errors = new ObservableCollection<ValidationError>
        {
            new(new ExceptionValidationRule(), new object(), null, null),
        };

        var ro = new ReadOnlyObservableCollection<ValidationError>(errors);

        var result = converter.Convert(
            value: ro,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            converter.ConvertBack(
                value: "anything",
                targetType: typeof(ReadOnlyObservableCollection<ValidationError>),
                parameter: null,
                culture: CultureInfo.InvariantCulture));
    }
}