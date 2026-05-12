namespace Atc.Wpf.Sample.SamplesWpf.ValueConverters;

public partial class LogCategoryTypeToColorValueConverterSampleViewModel : ViewModelBase
{
    public LogCategoryTypeToColorValueConverterSampleViewModel()
    {
        Entries = new ObservableCollection<LogCategoryTypePaletteEntry>(
            Enum.GetValues<LogCategoryType>()
                .Select(c => new LogCategoryTypePaletteEntry(c)));
    }

    public ObservableCollection<LogCategoryTypePaletteEntry> Entries { get; }

    [RelayCommand]
    private void RandomizeSecurity()
    {
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(3);
        var newColor = Color.FromRgb(bytes[0], bytes[1], bytes[2]);

        var security = Entries.FirstOrDefault(e => e.Category == LogCategoryType.Security);
        if (security is not null)
        {
            security.Color = newColor;
        }
    }

    [RelayCommand]
    private void ResetPalette()
    {
        LogCategoryTypeToColorValueConverter.ResetToDefaults();
        foreach (var entry in Entries)
        {
            entry.RefreshFromPalette();
        }
    }
}