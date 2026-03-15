namespace Atc.Wpf.Sample.SamplesWpfControls.ColorEditing;

public partial class WellKnownColorPickerDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Show Only Basic Colors", "Appearance", 1)]
    [ObservableProperty]
    private bool showOnlyBasicColors = true;
}