namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

[ValueConversion(typeof(bool), typeof(LabelPosition))]
public sealed class BoolToLabelPositionValueConverter : IValueConverter
{
    public static readonly BoolToLabelPositionValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is true
            ? LabelPosition.Right
            : LabelPosition.Left;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is LabelPosition.Right;
}