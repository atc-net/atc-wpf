namespace Atc.Wpf.Network.ValueConverters;

/// <summary>
/// ValueConverter: IPStatus To Localized Description.
/// </summary>
[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
[ValueConversion(typeof(IPStatus), typeof(string))]
public class IpStatusToLocalizedDescriptionValueConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared instance of the converter.
    /// </summary>
    public static readonly IpStatusToLocalizedDescriptionValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not IPStatus status)
        {
            return Binding.DoNothing;
        }

        try
        {
            return status.GetLocalizedDescription();
        }
        catch
        {
            return IPStatus.Unknown.GetLocalizedDescription();
        }
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}