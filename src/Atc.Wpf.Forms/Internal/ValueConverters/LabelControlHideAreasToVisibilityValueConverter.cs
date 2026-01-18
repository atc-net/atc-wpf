namespace Atc.Wpf.Forms.Internal.ValueConverters;

/// <summary>
/// ValueConverter: Label-Control HideAreas To Visibility.
/// </summary>
[ValueConversion(typeof(LabelControlHideAreasType), typeof(Visibility))]
internal sealed class LabelControlHideAreasToVisibilityValueConverter : IValueConverter
{
    public static readonly LabelControlHideAreasToVisibilityValueConverter Instance = new();

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
            return Visibility.Visible;
        }

        return !currentHideAreasType.HasFlag(requiredHideAreasType)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}