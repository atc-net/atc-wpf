// ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
// ReSharper disable InvertIf
// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// MultiValueConverter: Multi Bool To Visibility-Visible.
/// </summary>
[ValueConversion(typeof(List<bool>), typeof(Visibility))]
public sealed class MultiBoolToVisibilityVisibleValueConverter : IMultiValueConverter
{
    /// <summary>
    /// Gets a static default instance of <see cref="MultiBoolToVisibilityVisibleValueConverter"/>.
    /// </summary>
    public static readonly MultiBoolToVisibilityVisibleValueConverter Instance = new();

    /// <summary>
    /// Gets or sets the boolean operator used to combine the values when no operator is passed as the converter parameter.
    /// </summary>
    public BooleanOperatorType DefaultOperator { get; set; } = BooleanOperatorType.AND;

    /// <inheritdoc />
    public object Convert(
        object?[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        var operatorType = BooleanOperatorTypeResolver.Resolve(
            parameter,
            DefaultOperator);

        var boolValues = values.Select(v => v is true);

        return operatorType switch
        {
            BooleanOperatorType.AND => boolValues.All(b => b)
                ? Visibility.Visible
                : Visibility.Collapsed,
            BooleanOperatorType.OR => boolValues.Any(b => b)
                ? Visibility.Visible
                : Visibility.Collapsed,
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