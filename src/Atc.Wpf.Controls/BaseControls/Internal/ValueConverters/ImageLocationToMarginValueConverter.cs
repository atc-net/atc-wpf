namespace Atc.Wpf.Controls.BaseControls.Internal.ValueConverters;

/// <summary>
/// ValueConverter: ImageLocation to Thickness (Margin).
/// </summary>
[ValueConversion(typeof(ImageLocation), typeof(Thickness))]
internal class ImageLocationToMarginValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ImageLocation imageLocation)
        {
            throw new InvalidEnumArgumentException(nameof(value), 0, typeof(ImageLocation));
        }

        return imageLocation switch
        {
            ImageLocation.Left => new Thickness(0, 0, 5, 0),
            ImageLocation.Top => new Thickness(0, 0, 0, 5),
            ImageLocation.Right => new Thickness(5, 0, 0, 0),
            ImageLocation.Bottom => new Thickness(0, 5, 0, 0),
            ImageLocation.Center => new Thickness(0, 0, 0, 0),
            _ => throw new SwitchCaseDefaultException(imageLocation),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}