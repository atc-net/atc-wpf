namespace Atc.Wpf.Forms;

/// <summary>
/// Base class for labeled numeric input controls, adding the up/down button settings.
/// </summary>
public partial class LabelNumberControl : LabelControl, ILabelNumberControl
{
    [DependencyProperty(
        DefaultValue = ButtonsAlignmentType.Right,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure)]
    private ButtonsAlignmentType buttonsAlignment;

    [DependencyProperty(DefaultValue = false)]
    private bool hideUpDownButtons;
}