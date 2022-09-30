namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

/// <summary>
/// ValueConverter: Label-Control Orientation To Information-Icon Padding (Thickness).
/// </summary>
[ValueConversion(typeof(Orientation), typeof(Thickness))]
internal class LabelControlOrientationToAsteriskPaddingValueConverter : IValueConverter
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

        return orientation switch
        {
            Orientation.Horizontal => new Thickness(0, 7, 0, 0),
            Orientation.Vertical => new Thickness(0, 3, 0, 0),
            _ => throw new SwitchCaseDefaultException(orientation),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}