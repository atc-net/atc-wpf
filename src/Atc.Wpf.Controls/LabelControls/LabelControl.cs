namespace Atc.Wpf.Controls.LabelControls;

public class LabelControl : LabelControlBase, ILabelControl
{
    public static readonly DependencyProperty ShowAsteriskOnMandatoryProperty = DependencyProperty.Register(
        nameof(ShowAsteriskOnMandatory),
        typeof(bool),
        typeof(LabelControl),
        new PropertyMetadata(defaultValue: BooleanBoxes.TrueBox));

    public bool ShowAsteriskOnMandatory
    {
        get => (bool)GetValue(ShowAsteriskOnMandatoryProperty);
        set => SetValue(ShowAsteriskOnMandatoryProperty, value);
    }

    public static readonly DependencyProperty IsMandatoryProperty = DependencyProperty.Register(
        nameof(IsMandatory),
        typeof(bool),
        typeof(LabelControl),
        new PropertyMetadata(defaultValue: BooleanBoxes.FalseBox));

    public bool IsMandatory
    {
        get => (bool)GetValue(IsMandatoryProperty);
        set => SetValue(IsMandatoryProperty, value);
    }

    public static readonly DependencyProperty MandatoryColorProperty = DependencyProperty.Register(
        nameof(MandatoryColor),
        typeof(SolidColorBrush),
        typeof(LabelControl),
        new PropertyMetadata(new SolidColorBrush(Colors.Red)));

    public SolidColorBrush MandatoryColor
    {
        get => (SolidColorBrush)GetValue(MandatoryColorProperty);
        set => SetValue(MandatoryColorProperty, value);
    }

    public static readonly DependencyProperty ValidationColorProperty = DependencyProperty.Register(
        nameof(ValidationColor),
        typeof(SolidColorBrush),
        typeof(LabelControl),
        new PropertyMetadata(Application.Current?.Resources["AtcApps.Brushes.Control.Validation"] as SolidColorBrush ?? new SolidColorBrush(Colors.Red)));

    public SolidColorBrush ValidationColor
    {
        get => (SolidColorBrush)GetValue(ValidationColorProperty);
        set => SetValue(ValidationColorProperty, value);
    }

    public static readonly DependencyProperty ValidationTextProperty = DependencyProperty.Register(
        nameof(ValidationText),
        typeof(string),
        typeof(LabelControl),
        new PropertyMetadata(default(string)));

    public string ValidationText
    {
        get => (string)GetValue(ValidationTextProperty);
        set => SetValue(ValidationTextProperty, value);
    }

    public virtual bool IsValid() => true;
}