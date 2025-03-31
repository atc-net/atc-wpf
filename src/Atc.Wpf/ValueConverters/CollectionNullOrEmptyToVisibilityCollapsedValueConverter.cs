namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: ICollection Null Or Empty To Visibility-Collapsed.
/// </summary>
[ValueConversion(typeof(ICollection), typeof(Visibility))]
public sealed class CollectionNullOrEmptyToVisibilityCollapsedValueConverter : IValueConverter
{
    public static readonly CollectionNullOrEmptyToVisibilityCollapsedValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not ICollection collectionValue || collectionValue.Count == 0
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}