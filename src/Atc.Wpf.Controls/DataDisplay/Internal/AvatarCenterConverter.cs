#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Converts avatar size to center point for ellipse geometry.
/// </summary>
internal sealed class AvatarCenterConverter : IValueConverter
{
    public static readonly AvatarCenterConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is double size)
        {
            var center = size / 2;
            return new Point(center, center);
        }

        return new Point(20, 20); // Default for Medium size
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException();
}