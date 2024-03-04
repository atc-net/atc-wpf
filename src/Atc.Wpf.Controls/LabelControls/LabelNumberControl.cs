namespace Atc.Wpf.Controls.LabelControls;

public class LabelNumberControl : LabelControl, ILabelNumberControl
{
    public static readonly DependencyProperty ButtonsAlignmentProperty = DependencyProperty.Register(
        nameof(ButtonsAlignment),
        typeof(ButtonsAlignment),
        typeof(LabelNumberControl),
        new FrameworkPropertyMetadata(
            ButtonsAlignment.Right,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public ButtonsAlignment ButtonsAlignment
    {
        get => (ButtonsAlignment)GetValue(ButtonsAlignmentProperty);
        set => SetValue(ButtonsAlignmentProperty, value);
    }

    public static readonly DependencyProperty HideUpDownButtonsProperty = DependencyProperty.Register(
        nameof(HideUpDownButtons),
        typeof(bool),
        typeof(LabelNumberControl),
        new PropertyMetadata(default(bool)));

    public bool HideUpDownButtons
    {
        get => (bool)GetValue(HideUpDownButtonsProperty);
        set => SetValue(HideUpDownButtonsProperty, value);
    }
}