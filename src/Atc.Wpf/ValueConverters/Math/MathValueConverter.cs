// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// MathConverter provides a value converter which can be used for math operations.
/// It can be used for normal binding or multi binding as well.
/// If it is used for normal binding the given parameter will be used as operands with the selected operation.
/// If it is used for multi binding then the first and second binding will be used as operands with the selected operation.
/// This class cannot be inherited.
/// </summary>
[ValueConversion(typeof(object), typeof(object))]
public sealed class MathValueConverter : IValueConverter, IMultiValueConverter
{
    public MathOperation Operation { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DoConvert(value, parameter, Operation);
    }

    public object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values is null
            ? Binding.DoNothing
            : DoConvert(values.ElementAtOrDefault(0), values.ElementAtOrDefault(1), Operation);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }

    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        return targetTypes.Select(_ => DependencyProperty.UnsetValue).ToArray();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK - errorHandler will handle it")]
    private static object? DoConvert(object? firstValue, object? secondValue, MathOperation operation)
    {
        if (firstValue is null ||
            secondValue is null ||
            firstValue == DependencyProperty.UnsetValue ||
            secondValue == DependencyProperty.UnsetValue ||
            firstValue == DBNull.Value ||
            secondValue == DBNull.Value)
        {
            return Binding.DoNothing;
        }

        try
        {
            var value1 = (firstValue as double?).GetValueOrDefault(System.Convert.ToDouble(firstValue, CultureInfo.InvariantCulture));
            var value2 = (secondValue as double?).GetValueOrDefault(System.Convert.ToDouble(secondValue, CultureInfo.InvariantCulture));

            switch (operation)
            {
                case MathOperation.Add:
                    return value1 + value2;
                case MathOperation.Divide:
                    if (value2 > 0)
                    {
                        return value1 / value2;
                    }

                    return Binding.DoNothing;

                case MathOperation.Multiply:
                    return value1 * value2;
                case MathOperation.Subtract:
                    return value1 - value2;
                default:
                    return Binding.DoNothing;
            }
        }
        catch
        {
            return Binding.DoNothing;
        }
    }
}