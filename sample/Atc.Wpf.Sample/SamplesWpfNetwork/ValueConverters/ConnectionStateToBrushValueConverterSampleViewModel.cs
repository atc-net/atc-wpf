namespace Atc.Wpf.Sample.SamplesWpfNetwork.ValueConverters;

public partial class ConnectionStateToBrushValueConverterSampleViewModel : ViewModelBase
{
    public ConnectionStateToBrushValueConverterSampleViewModel()
    {
        Entries = new ObservableCollection<ConnectionStatePaletteEntry>(
            Enum.GetValues<Atc.Network.ConnectionState>()
                .Select(s => new ConnectionStatePaletteEntry(s)));
    }

    public ObservableCollection<ConnectionStatePaletteEntry> Entries { get; }

    [RelayCommand]
    private void RandomizePulse()
    {
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(3);
        var newColor = Color.FromRgb(bytes[0], bytes[1], bytes[2]);

        var pulse = Entries.FirstOrDefault(e => e.State == Atc.Network.ConnectionState.Pulse);
        if (pulse is not null)
        {
            pulse.Color = newColor;
        }
    }

    [RelayCommand]
    private void ResetPalette()
    {
        ConnectionStateToColorValueConverter.ResetToDefaults();
        foreach (var entry in Entries)
        {
            entry.RefreshFromPalette();
        }
    }
}