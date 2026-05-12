// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class BoolToObjectValueConverterTests
{
    [StaFact]
    public void Convert_True_ReturnsTrueValue()
    {
        var converter = new BoolToObjectValueConverter
        {
            TrueValue = "Online",
            FalseValue = "Offline",
        };

        var result = ((IValueConverter)converter).Convert(
            value: true,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.Equal("Online", result);
    }

    [StaFact]
    public void Convert_False_ReturnsFalseValue()
    {
        var converter = new BoolToObjectValueConverter
        {
            TrueValue = "Online",
            FalseValue = "Offline",
        };

        var result = ((IValueConverter)converter).Convert(
            value: false,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.Equal("Offline", result);
    }

    [StaFact]
    public void Convert_Null_ReturnsFalseValue()
    {
        var converter = new BoolToObjectValueConverter
        {
            TrueValue = "Yes",
            FalseValue = "No",
        };

        var result = ((IValueConverter)converter).Convert(
            value: null,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.Equal("No", result);
    }

    [StaFact]
    public void Convert_NonBool_ReturnsFalseValue()
    {
        var converter = new BoolToObjectValueConverter
        {
            TrueValue = 1,
            FalseValue = 0,
        };

        var result = ((IValueConverter)converter).Convert(
            value: "NotABool",
            targetType: typeof(int),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.Equal(0, result);
    }

    [StaFact]
    public void Convert_UnsetValues_ReturnsNull()
    {
        var converter = new BoolToObjectValueConverter();

        var trueResult = ((IValueConverter)converter).Convert(true, typeof(object), null, CultureInfo.InvariantCulture);
        var falseResult = ((IValueConverter)converter).Convert(false, typeof(object), null, CultureInfo.InvariantCulture);

        Assert.Null(trueResult);
        Assert.Null(falseResult);
    }

    [StaFact]
    public void Convert_AcceptsBrushes()
    {
        var converter = new BoolToObjectValueConverter
        {
            TrueValue = Brushes.Green,
            FalseValue = Brushes.Red,
        };

        var enabledResult = ((IValueConverter)converter).Convert(true, typeof(Brush), null, CultureInfo.InvariantCulture);
        var disabledResult = ((IValueConverter)converter).Convert(false, typeof(Brush), null, CultureInfo.InvariantCulture);

        Assert.Same(Brushes.Green, enabledResult);
        Assert.Same(Brushes.Red, disabledResult);
    }

    [StaFact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var converter = new BoolToObjectValueConverter();

        var exception = Record.Exception(() => ((IValueConverter)converter).ConvertBack(
            value: "Online",
            targetType: typeof(bool),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}