namespace Atc.Wpf.Forms.Internal.ValueConverters;

/// <summary>
/// ValueConverter: Label-Control HideAreas To bool.
/// </summary>
[ValueConversion(typeof(LabelControlHideAreasType), typeof(bool))]
internal sealed class LabelControlHideAreasToBoolValueConverter : IValueConverter
{
    public static readonly LabelControlHideAreasToBoolValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not LabelControlHideAreasType currentHideAreasType ||
            parameter is not LabelControlHideAreasType requiredHideAreasType)
        {
            throw new InvalidEnumArgumentException(nameof(value), 0, typeof(LabelControlHideAreasType));
        }

        if (requiredHideAreasType == LabelControlHideAreasType.None)
        {
            return true;
        }

        return !currentHideAreasType.HasFlag(requiredHideAreasType);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}