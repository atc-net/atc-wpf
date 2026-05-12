namespace Atc.Wpf.Sample.SamplesWpfNetwork.ValueConverters;

/// <summary>
/// Row view-model for the ConnectionState-palette samples.
/// Exposes a two-way <see cref="Color"/> property whose setter pushes the new value into
/// <see cref="ConnectionStateToColorValueConverter"/>. The <see cref="Brush"/> and
/// <see cref="HexCode"/> derived properties re-notify so bindings update.
/// </summary>
public class ConnectionStatePaletteEntry : ViewModelBase
{
    private Color color;

    public ConnectionStatePaletteEntry(Atc.Network.ConnectionState state)
    {
        State = state;
        color = ConnectionStateToColorValueConverter.GetColor(state);
    }

    public Atc.Network.ConnectionState State { get; }

    public Color Color
    {
        get => color;
        set
        {
            if (color == value)
            {
                return;
            }

            color = value;
            ConnectionStateToColorValueConverter.SetColor(State, value);
            RaiseChangeNotifications();
        }
    }

    public SolidColorBrush Brush
        => ConnectionStateToBrushValueConverter.GetBrush(State);

    public string HexCode
        => Color.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Refreshes <see cref="Color"/> from the current static palette without writing back.
    /// Use after an external palette change (e.g. ResetToDefaults) to pull the new color.
    /// </summary>
    public void RefreshFromPalette()
    {
        var paletteColor = ConnectionStateToColorValueConverter.GetColor(State);
        if (color == paletteColor)
        {
            return;
        }

        color = paletteColor;
        RaiseChangeNotifications();
    }

    private void RaiseChangeNotifications()
    {
        OnPropertyChanged(nameof(Color));
        OnPropertyChanged(nameof(Brush));
        OnPropertyChanged(nameof(HexCode));
    }
}