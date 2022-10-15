namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

internal class LabelControlHideAreasToVisibilityValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not LabelControlHideAreasType currentHideAreasType)
        {
            throw new InvalidEnumArgumentException(nameof(value), 0, typeof(LabelControlHideAreasType));
        }

        if (parameter is not LabelControlHideAreasType requiredHideAreasType)
        {
            throw new InvalidEnumArgumentException(nameof(value), 0, typeof(LabelControlHideAreasType));
        }

        if (requiredHideAreasType == LabelControlHideAreasType.None)
        {
            return Visibility.Visible;
        }

        return !currentHideAreasType.HasFlag(requiredHideAreasType)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}