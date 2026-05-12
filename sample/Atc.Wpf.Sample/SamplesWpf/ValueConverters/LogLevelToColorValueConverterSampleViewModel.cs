namespace Atc.Wpf.Sample.SamplesWpf.ValueConverters;

public partial class LogLevelToColorValueConverterSampleViewModel : ViewModelBase
{
    public LogLevelToColorValueConverterSampleViewModel()
    {
        Entries = new ObservableCollection<LogLevelPaletteEntry>(
            new[]
            {
                LogLevel.Trace,
                LogLevel.Debug,
                LogLevel.Information,
                LogLevel.Warning,
                LogLevel.Error,
                LogLevel.Critical,
            }.Select(l => new LogLevelPaletteEntry(l)));
    }

    public ObservableCollection<LogLevelPaletteEntry> Entries { get; }

    [RelayCommand]
    private void RandomizeWarning()
    {
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(3);
        var newColor = Color.FromRgb(bytes[0], bytes[1], bytes[2]);

        var warning = Entries.FirstOrDefault(e => e.Level == LogLevel.Warning);
        if (warning is not null)
        {
            warning.Color = newColor;
        }
    }

    [RelayCommand]
    private void ResetPalette()
    {
        LogLevelToColorValueConverter.ResetToDefaults();
        foreach (var entry in Entries)
        {
            entry.RefreshFromPalette();
        }
    }
}