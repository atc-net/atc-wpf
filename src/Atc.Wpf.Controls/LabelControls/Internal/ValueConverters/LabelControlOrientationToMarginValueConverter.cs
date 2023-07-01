namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

/// <summary>
/// ValueConverter: Label-Control Orientation To Margin (Thickness).
/// </summary>
[ValueConversion(typeof(Orientation), typeof(Thickness))]
internal sealed class LabelControlOrientationToMarginValueConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return new Thickness(0);
        }

        if (value is not Orientation orientation)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Orientation)");
        }

        var spacingType = LabelControlSpacingType.Inside;
        if (parameter is not null &&
            nameof(LabelControlSpacingType).Equals(parameter.GetType().Name, StringComparison.Ordinal))
        {
            spacingType = Enum<LabelControlSpacingType>.Parse(parameter.ToString()!, ignoreCase: false);
        }

        return orientation switch
        {
            Orientation.Horizontal => spacingType == LabelControlSpacingType.Inside
                ? new Thickness(0, 0, 10, 0)
                : new Thickness(0, 0, 10, 2),
            Orientation.Vertical => spacingType != LabelControlSpacingType.Inside
                ? new Thickness(0, 0, 0, 15)
                : new Thickness(0, 0, 0, 5),
            _ => throw new SwitchCaseDefaultException(orientation),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}