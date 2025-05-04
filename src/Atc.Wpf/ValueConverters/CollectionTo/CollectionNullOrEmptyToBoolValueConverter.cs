// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: ICollection Null Or Empty To Bool.
/// </summary>
[ValueConversion(typeof(ICollection), typeof(bool))]
public sealed class CollectionNullOrEmptyToBoolValueConverter : IValueConverter
{
    public static readonly CollectionNullOrEmptyToBoolValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is not ICollection collectionValue ||
           collectionValue.Count == 0;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}