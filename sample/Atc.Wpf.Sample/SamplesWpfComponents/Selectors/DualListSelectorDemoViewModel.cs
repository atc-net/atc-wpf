namespace Atc.Wpf.Sample.SamplesWpfComponents.Selectors;

public partial class DualListSelectorDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Layout Mode", "Layout", 1)]
    [ObservableProperty]
    private DualListSelectorLayoutMode layoutMode = DualListSelectorLayoutMode.AvailableFirst;

    [PropertyDisplay("Show Reorder Buttons", "Buttons", 1)]
    [ObservableProperty]
    private bool showReorderButtons = true;

    [PropertyDisplay("Show Transfer All Buttons", "Buttons", 2)]
    [ObservableProperty]
    private bool showTransferAllButtons = true;

    [PropertyDisplay("Auto Recalculate Sort Order", "Behavior", 1)]
    [ObservableProperty]
    private bool autoRecalculateSortOrder = true;

    [PropertyDisplay("Show Filter", "Behavior", 2)]
    [ObservableProperty]
    private bool showFilter = true;

    [PropertyDisplay("Allow Drag Drop", "Behavior", 3)]
    [ObservableProperty]
    private bool allowDragDrop = true;

    [PropertyDisplay("Allow Multi Select", "Behavior", 4)]
    [ObservableProperty]
    private bool allowMultiSelect = true;

    [PropertyDisplay("Show Item Count", "Behavior", 5)]
    [ObservableProperty]
    private bool showItemCount = true;
}