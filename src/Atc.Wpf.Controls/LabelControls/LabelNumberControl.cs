namespace Atc.Wpf.Controls.LabelControls;

public class LabelNumberControl : LabelControl, ILabelNumberControl
{
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