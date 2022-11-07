namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

/// <summary>
/// ValueConverter: Label-Control HideAreas To string of rows: "[0],[1],[2]" or "[0],[1],[2],[3]".
/// </summary>
[ValueConversion(typeof(LabelControlHideAreasType), typeof(string))]
public class LabelControlHideAreasForValidationToGridExRowsValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not LabelControlHideAreasType currentHideAreasType)
        {
            throw new InvalidEnumArgumentException(nameof(value), 0, typeof(LabelControlHideAreasType));
        }

        if (parameter is not Orientation orientation)
        {
            throw new InvalidEnumArgumentException(nameof(value), 0, typeof(LabelControlHideAreasType));
        }

        if (currentHideAreasType.HasFlag(LabelControlHideAreasType.Validation))
        {
            return orientation == Orientation.Horizontal
                ? "Auto,Auto"
                : "Auto,Auto,Auto";
        }

        return orientation == Orientation.Horizontal
            ? "Auto,Auto,10"
            : "Auto,Auto,Auto,10";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}