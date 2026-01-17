namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelNumberControl : LabelControl, ILabelNumberControl
{
    [DependencyProperty(
        DefaultValue = ButtonsAlignmentType.Right,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure)]
    private ButtonsAlignmentType buttonsAlignment;

    [DependencyProperty(DefaultValue = false)]
    private bool hideUpDownButtons;
}