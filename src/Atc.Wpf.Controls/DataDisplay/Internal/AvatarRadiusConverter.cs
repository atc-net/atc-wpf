#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Converts avatar size to radius (half of size).
/// </summary>
internal sealed class AvatarRadiusConverter : IValueConverter
{
    public static readonly AvatarRadiusConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is double size)
        {
            return size / 2;
        }

        return 20d; // Default for Medium size
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException();
}