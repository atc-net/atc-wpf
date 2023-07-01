namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

[ValueConversion(typeof(LabelControlHideAreasType), typeof(bool))]
internal sealed class LabelControlHideAreasToShowToolTipValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not LabelControlHideAreasType currentHideAreasType)
        {
            throw new InvalidEnumArgumentException(nameof(value), 0, typeof(LabelControlHideAreasType));
        }

        return currentHideAreasType.HasFlag(LabelControlHideAreasType.Validation);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}