// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters.Math;

public sealed class MathValueConverterTests
{
    [Theory]
    [InlineData(MathOperation.Add, 5d, 3d, 8d)]
    [InlineData(MathOperation.Subtract, 10d, 4d, 6d)]
    [InlineData(MathOperation.Multiply, 6d, 7d, 42d)]
    [InlineData(MathOperation.Divide, 20d, 4d, 5d)]
    public void Convert_single_binding_uses_value_and_parameter_as_operands(
        MathOperation operation,
        double value,
        double parameter,
        double expected)
    {
        var converter = new MathValueConverter { Operation = operation };

        var actual = converter.Convert(
            value,
            typeof(double),
            parameter,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(MathOperation.Add, 1d, 2d, 3d)]
    [InlineData(MathOperation.Subtract, 9d, 4d, 5d)]
    [InlineData(MathOperation.Multiply, 3d, 5d, 15d)]
    [InlineData(MathOperation.Divide, 12d, 4d, 3d)]
    public void Convert_multi_binding_uses_first_two_values_as_operands(
        MathOperation operation,
        double first,
        double second,
        double expected)
    {
        var converter = new MathValueConverter { Operation = operation };

        var actual = converter.Convert(
            new object[] { first, second, "extra-ignored" },
            typeof(double),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Convert_multi_binding_returns_BindingDoNothing_for_null_values_array()
    {
        var converter = new MathValueConverter { Operation = MathOperation.Add };

        var actual = converter.Convert(
            values: null,
            typeof(double),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(Binding.DoNothing, actual);
    }

    [Fact]
    public void Convert_returns_BindingDoNothing_when_division_by_non_positive()
    {
        var converter = new MathValueConverter { Operation = MathOperation.Divide };

        var actual = converter.Convert(
            10d,
            typeof(double),
            parameter: 0d,
            CultureInfo.InvariantCulture);

        Assert.Same(Binding.DoNothing, actual);
    }

    [Theory]
    [InlineData(null, 5d)]
    [InlineData(5d, null)]
    [InlineData(null, null)]
    public void Convert_returns_BindingDoNothing_when_either_operand_is_null(
        object? first,
        object? second)
    {
        var converter = new MathValueConverter { Operation = MathOperation.Add };

        var actual = converter.Convert(
            first,
            typeof(double),
            second,
            CultureInfo.InvariantCulture);

        Assert.Same(Binding.DoNothing, actual);
    }

    [Fact]
    public void Convert_returns_BindingDoNothing_when_operands_cannot_be_converted_to_double()
    {
        var converter = new MathValueConverter { Operation = MathOperation.Add };

        var actual = converter.Convert(
            "not a number",
            typeof(double),
            parameter: 5d,
            CultureInfo.InvariantCulture);

        Assert.Same(Binding.DoNothing, actual);
    }

    [Fact]
    public void ConvertBack_single_returns_DependencyProperty_UnsetValue()
    {
        var converter = new MathValueConverter { Operation = MathOperation.Add };

        var actual = converter.ConvertBack(
            42d,
            typeof(double),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(DependencyProperty.UnsetValue, actual);
    }

    [Fact]
    public void ConvertBack_multi_returns_one_UnsetValue_per_targetType()
    {
        var converter = new MathValueConverter { Operation = MathOperation.Add };

        var actual = converter.ConvertBack(
            42d,
            [typeof(double), typeof(double), typeof(int)],
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(3, actual.Length);
        Assert.All(actual, x => Assert.Same(DependencyProperty.UnsetValue, x));
    }
}