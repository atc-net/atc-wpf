namespace Atc.Wpf.Controls.Tests.Selectors;

// Bindings need a public type, so the page view model used by the selector DataContext tests lives in its own file.
public sealed class SelectorPageViewModel : INotifyPropertyChanged
{
    private string selectedKey = string.Empty;
    private bool showOnlyBasicColors;
    private Brush? colorBrush;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string SelectedKey
    {
        get => selectedKey;
        set
        {
            selectedKey = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedKey)));
        }
    }

    public bool ShowOnlyBasicColors
    {
        get => showOnlyBasicColors;
        set
        {
            showOnlyBasicColors = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowOnlyBasicColors)));
        }
    }

    public Brush? ColorBrush
    {
        get => colorBrush;
        set
        {
            colorBrush = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ColorBrush)));
        }
    }
}