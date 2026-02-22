namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public class PopoverDemoViewModel : ViewModelBase
{
    private PopoverPlacement placement = PopoverPlacement.Bottom;
    private PopoverTriggerMode triggerMode = PopoverTriggerMode.Click;
    private bool showArrow = true;
    private bool isOpen;
    private int openDelay;
    private int closeDelay = 200;
    private double cornerRadius = 4;

    [PropertyDisplay("Placement", "Layout", 1)]
    public PopoverPlacement Placement
    {
        get => placement;
        set => Set(ref placement, value);
    }

    [PropertyDisplay("Trigger Mode", "Behavior", 1)]
    public PopoverTriggerMode TriggerMode
    {
        get => triggerMode;
        set => Set(ref triggerMode, value);
    }

    [PropertyDisplay("Show Arrow", "Appearance", 1)]
    public bool ShowArrow
    {
        get => showArrow;
        set => Set(ref showArrow, value);
    }

    [PropertyDisplay("Is Open", "Behavior", 2)]
    public bool IsOpen
    {
        get => isOpen;
        set => Set(ref isOpen, value);
    }

    [PropertyDisplay("Open Delay (ms)", "Behavior", 3)]
    [PropertyRange(0, 1000, 50)]
    public int OpenDelay
    {
        get => openDelay;
        set => Set(ref openDelay, value);
    }

    [PropertyDisplay("Close Delay (ms)", "Behavior", 4)]
    [PropertyRange(0, 1000, 50)]
    public int CloseDelay
    {
        get => closeDelay;
        set => Set(ref closeDelay, value);
    }

    [PropertyDisplay("Corner Radius", "Appearance", 2)]
    [PropertyRange(0, 20, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double CornerRadius
    {
        get => cornerRadius;
        set => Set(ref cornerRadius, value);
    }
}