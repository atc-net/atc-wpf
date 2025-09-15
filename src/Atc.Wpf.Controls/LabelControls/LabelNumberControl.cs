namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelNumberControl : LabelControl, ILabelNumberControl
{
    [DependencyProperty(
        DefaultValue = ButtonsAlignment.Right,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure)]
    private ButtonsAlignment buttonsAlignment;

    [DependencyProperty(DefaultValue = false)]
    private bool hideUpDownButtons;
}