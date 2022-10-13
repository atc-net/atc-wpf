// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

public class LabelControlHorizontalToWidthMultiValueConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        {
            return "0.3*,0.7*";
        }

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
            case SizeDefinitionType.Pixel when width < 10:
                throw new ConstraintException("Width in Pixel mode must be greater than 10.");
            case SizeDefinitionType.Pixel:
                return $"{width},*";
            case SizeDefinitionType.Percentage when width is < 0 or > 100:
                throw new ConstraintException("Width in Percentage mode must be between 0 and 100.");
            case SizeDefinitionType.Percentage:
            {
                var leftWidth = (double)width / 100;
                var rightWidth = (100 - (double)width) / 100;
                var twoColumnWidth = $"{leftWidth.ToString(GlobalizationConstants.EnglishCultureInfo)}*,{rightWidth.ToString(GlobalizationConstants.EnglishCultureInfo)}*";

                return twoColumnWidth;
            }

            default:
                throw new SwitchCaseDefaultException(sizeDefinition);
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}