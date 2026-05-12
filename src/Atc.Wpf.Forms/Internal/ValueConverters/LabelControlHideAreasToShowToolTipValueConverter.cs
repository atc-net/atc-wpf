namespace Atc.Wpf.Forms.Internal.ValueConverters;

/// <summary>
/// Internal converter used by labelled form controls to decide whether the validation tooltip
/// should be displayed: returns <see langword="true"/> when the
/// <see cref="LabelControlHideAreasType.Validation"/> flag is set on the bound value.
/// </summary>
[ValueConversion(typeof(LabelControlHideAreasType), typeof(bool))]
internal sealed class LabelControlHideAreasToShowToolTipValueConverter : IValueConverter
{
    public static readonly LabelControlHideAreasToShowToolTipValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not LabelControlHideAreasType currentHideAreasType)
        {
            throw new InvalidEnumArgumentException(nameof(value), 0, typeof(LabelControlHideAreasType));
        }

        return currentHideAreasType.HasFlag(LabelControlHideAreasType.Validation);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}