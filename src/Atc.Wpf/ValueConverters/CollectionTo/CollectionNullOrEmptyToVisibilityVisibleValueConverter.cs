// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: ICollection Null Or Empty To Visibility-Visible.
/// </summary>
[ValueConversion(typeof(ICollection), typeof(Visibility))]
public sealed class CollectionNullOrEmptyToVisibilityVisibleValueConverter : IValueConverter
{
    public static readonly CollectionNullOrEmptyToVisibilityVisibleValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not ICollection collectionValue || collectionValue.Count == 0
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}