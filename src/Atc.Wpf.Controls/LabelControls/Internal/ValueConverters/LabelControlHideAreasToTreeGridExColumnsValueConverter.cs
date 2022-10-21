namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

/// <summary>
/// ValueConverter: Label-Control HideAreas To string of columns: "[0],[1],[2]".
/// </summary>
[ValueConversion(typeof(LabelControlHideAreasType), typeof(string))]
internal class LabelControlHideAreasToTreeGridExColumnsValueConverter : IValueConverter
{
    [SuppressMessage("Design", "MA0076:Do not use implicit culture-sensitive ToString in interpolated strings", Justification = "OK.")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not LabelControlHideAreasType currentHideAreasType)
        {
            throw new InvalidEnumArgumentException(nameof(value), 0, typeof(LabelControlHideAreasType));
        }

        var widthLeft = 0;
        if (!currentHideAreasType.HasFlag(LabelControlHideAreasType.Asterisk))
        {
            widthLeft = 10;
        }

        var widthRight = 0;
        if (!currentHideAreasType.HasFlag(LabelControlHideAreasType.Information))
        {
            widthRight = 20;
        }

        return $"{widthLeft},*,{widthRight}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}