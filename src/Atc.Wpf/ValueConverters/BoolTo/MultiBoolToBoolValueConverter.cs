// ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
// // ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Multi Bool To Bool.
/// </summary>
[ValueConversion(typeof(List<bool>), typeof(bool))]
public sealed class MultiBoolToBoolValueConverter : IMultiValueConverter
{
    public static readonly MultiBoolToBoolValueConverter Instance = new();

    public BooleanOperatorType DefaultOperator { get; set; } = BooleanOperatorType.AND;

    /// <inheritdoc />
    public object Convert(
        object?[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        var operatorType = BooleanOperatorTypeResolver.Resolve(
            parameter,
            DefaultOperator);

        var boolValues = values.Select(v => v is true);

        return operatorType switch
        {
            BooleanOperatorType.AND => boolValues.All(b => b),
            BooleanOperatorType.OR => boolValues.Any(b => b),
            _ => throw new SwitchCaseDefaultException(nameof(operatorType)),
        };
    }

    /// <inheritdoc />
    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}