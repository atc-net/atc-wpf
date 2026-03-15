namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class ChipDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Variant", "Appearance", 1)]
    [ObservableProperty]
    private ChipVariant variant = ChipVariant.Default;

    [PropertyDisplay("Size", "Appearance", 2)]
    [ObservableProperty]
    private ChipSize size = ChipSize.Medium;

    [PropertyDisplay("Is Selectable", "Behavior", 1)]
    [ObservableProperty]
    private bool isSelectable;

    [PropertyDisplay("Is Selected", "Behavior", 2)]
    [ObservableProperty]
    private bool chipIsSelected;

    [PropertyDisplay("Can Remove", "Behavior", 3)]
    [ObservableProperty]
    private bool canRemove;

    [PropertyDisplay("Is Clickable", "Behavior", 4)]
    [ObservableProperty]
    private bool isClickable = true;

    [PropertyDisplay("Corner Radius", "Appearance", 3)]
    [PropertyRange(0, 30, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double cornerRadius = 16;

    [PropertyDisplay("Content", "Content", 1)]
    [ObservableProperty]
    private string content = "Sample Chip";
}