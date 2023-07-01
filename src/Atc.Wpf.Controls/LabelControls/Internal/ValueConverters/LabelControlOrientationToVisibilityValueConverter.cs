namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

/// <summary>
/// ValueConverter: Label-Control Orientation To Visibility.
/// </summary>
[ValueConversion(typeof(Orientation), typeof(Visibility))]
internal sealed class LabelControlOrientationToVisibilityValueConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return Visibility.Visible;
        }

        if (value is not Orientation orientation)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Orientation)");
        }

        var displayOnOrientation = Orientation.Horizontal;
        if (parameter is Orientation parameterOrientation)
        {
            displayOnOrientation = parameterOrientation;
        }

        return orientation == displayOnOrientation
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}