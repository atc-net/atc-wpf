// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace Atc.Wpf.Forms.Internal.ValueConverters;

/// <summary>
/// MultiValueConverter: Label-Control Width To string of columns: "[0],[1]".
/// </summary>
/// <remarks>
/// Column structure is always "[specifiedWidth],*".
/// When LabelPosition=Left: Column 0 = Label, Column 1 = Input.
/// When LabelPosition=Right: Column 0 = Input, Column 1 = Label.
/// The elements swap positions, not the column widths.
/// </remarks>
[ValueConversion(typeof(int), typeof(string))]
internal sealed class LabelControlHorizontalToWidthMultiValueConverter : IMultiValueConverter
{
    public static readonly LabelControlHorizontalToWidthMultiValueConverter Instance = new();

    public object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        var width = 0;
        var sizeDefinition = SizeDefinitionType.None;

        foreach (var value in values)
        {
            switch (value)
            {
                case int intValue:
                    width = intValue;
                    break;
                case SizeDefinitionType sizeDefinitionValue:
                    sizeDefinition = sizeDefinitionValue;
                    break;
            }
        }

        switch (sizeDefinition)
        {
            case SizeDefinitionType.None:
                return $"{width.ToString(GlobalizationConstants.EnglishCultureInfo)},*";
            case SizeDefinitionType.Pixel when width < 10:
                throw new ConstraintException("Width in Pixel mode must be greater than 10.");
            case SizeDefinitionType.Pixel:
                return $"{width.ToString(GlobalizationConstants.EnglishCultureInfo)},*";
            case SizeDefinitionType.Percentage when width is < 0 or > 100:
                throw new ConstraintException("Width in Percentage mode must be between 0 and 100.");
            case SizeDefinitionType.Percentage:
                {
                    var leftWidth = (double)width / 100;
                    var rightWidth = (100 - (double)width) / 100;
                    return $"{leftWidth.ToString(GlobalizationConstants.EnglishCultureInfo)}*,{rightWidth.ToString(GlobalizationConstants.EnglishCultureInfo)}*";
                }

            case SizeDefinitionType.Auto:
                return "Auto,*";
            default:
                throw new SwitchCaseDefaultException(sizeDefinition);
        }
    }

    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}