namespace Atc.Wpf.Network.ValueConverters;

/// <summary>
/// ValueConverter: NetworkQualityCategoryTypeToResourceImage.
/// </summary>
[ValueConversion(typeof(NetworkQualityCategoryType), typeof(BitmapImage))]
public class NetworkQualityCategoryTypeToResourceImageValueConverter : IValueConverter
{
    [SuppressMessage("S1075", "S1075:URIs should not be hardcoded", Justification = "Pack URIs are required for WPF embedded resources.")]
    private const string ResourceBasePath = "pack://application:,,,/Atc.Wpf.Network;component/Resources/Images/";

    private static readonly Lazy<BitmapImage> VeryPoorImage = new(() => CreateFrozenBitmapImage("0_very_poor.png"));
    private static readonly Lazy<BitmapImage> PoorImage = new(() => CreateFrozenBitmapImage("1_poor.png"));
    private static readonly Lazy<BitmapImage> FairImage = new(() => CreateFrozenBitmapImage("2_fair.png"));
    private static readonly Lazy<BitmapImage> GoodImage = new(() => CreateFrozenBitmapImage("3_good.png"));
    private static readonly Lazy<BitmapImage> VeryGoodImage = new(() => CreateFrozenBitmapImage("4_very_good.png"));
    private static readonly Lazy<BitmapImage> ExcellentImage = new(() => CreateFrozenBitmapImage("5_excellent.png"));
    private static readonly Lazy<BitmapImage> PerfectImage = new(() => CreateFrozenBitmapImage("6_perfect.png"));

    public static readonly NetworkQualityCategoryTypeToResourceImageValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not NetworkQualityCategoryType networkQualityCategoryType)
        {
            throw new ArgumentException($"value is not {typeof(NetworkQualityCategoryType)}", nameof(value));
        }

        return networkQualityCategoryType switch
        {
            NetworkQualityCategoryType.None => Binding.DoNothing,
            NetworkQualityCategoryType.VeryPoor => VeryPoorImage.Value,
            NetworkQualityCategoryType.Poor => PoorImage.Value,
            NetworkQualityCategoryType.Fair => FairImage.Value,
            NetworkQualityCategoryType.Good => GoodImage.Value,
            NetworkQualityCategoryType.VeryGood => VeryGoodImage.Value,
            NetworkQualityCategoryType.Excellent => ExcellentImage.Value,
            NetworkQualityCategoryType.Perfect => PerfectImage.Value,
            _ => throw new SwitchCaseDefaultException(networkQualityCategoryType),
        };
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException();

    private static BitmapImage CreateFrozenBitmapImage(string fileName)
    {
        var bitmap = new BitmapImage(new Uri($"{ResourceBasePath}{fileName}", UriKind.Absolute));
        bitmap.Freeze();
        return bitmap;
    }
}