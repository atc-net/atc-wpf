namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class PopoverDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Placement", "Layout", 1)]
    [ObservableProperty]
    private PopoverPlacement placement = PopoverPlacement.Bottom;

    [PropertyDisplay("Trigger Mode", "Behavior", 1)]
    [ObservableProperty]
    private PopoverTriggerMode triggerMode = PopoverTriggerMode.Click;

    [PropertyDisplay("Show Arrow", "Appearance", 1)]
    [ObservableProperty]
    private bool showArrow = true;

    [PropertyDisplay("Is Open", "Behavior", 2)]
    [ObservableProperty]
    private bool isOpen;

    [PropertyDisplay("Open Delay (ms)", "Behavior", 3)]
    [PropertyRange(0, 1000, 50)]
    [ObservableProperty]
    private int openDelay;

    [PropertyDisplay("Close Delay (ms)", "Behavior", 4)]
    [PropertyRange(0, 1000, 50)]
    [ObservableProperty]
    private int closeDelay = 200;

    [PropertyDisplay("Corner Radius", "Appearance", 2)]
    [PropertyRange(0, 20, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double cornerRadius = 4;
}