namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public class ChipDemoViewModel : ViewModelBase
{
    private ChipVariant variant = ChipVariant.Default;
    private ChipSize size = ChipSize.Medium;
    private bool isSelectable;
    private bool chipIsSelected;
    private bool canRemove;
    private bool isClickable = true;
    private double cornerRadius = 16;
    private string content = "Sample Chip";

    [PropertyDisplay("Variant", "Appearance", 1)]
    public ChipVariant Variant
    {
        get => variant;
        set => Set(ref variant, value);
    }

    [PropertyDisplay("Size", "Appearance", 2)]
    public ChipSize Size
    {
        get => size;
        set => Set(ref size, value);
    }

    [PropertyDisplay("Is Selectable", "Behavior", 1)]
    public bool IsSelectable
    {
        get => isSelectable;
        set => Set(ref isSelectable, value);
    }

    [PropertyDisplay("Is Selected", "Behavior", 2)]
    public bool ChipIsSelected
    {
        get => chipIsSelected;
        set => Set(ref chipIsSelected, value);
    }

    [PropertyDisplay("Can Remove", "Behavior", 3)]
    public bool CanRemove
    {
        get => canRemove;
        set => Set(ref canRemove, value);
    }

    [PropertyDisplay("Is Clickable", "Behavior", 4)]
    public bool IsClickable
    {
        get => isClickable;
        set => Set(ref isClickable, value);
    }

    [PropertyDisplay("Corner Radius", "Appearance", 3)]
    [PropertyRange(0, 30, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double CornerRadius
    {
        get => cornerRadius;
        set => Set(ref cornerRadius, value);
    }

    [PropertyDisplay("Content", "Content", 1)]
    public string Content
    {
        get => content;
        set => Set(ref content, value);
    }
}