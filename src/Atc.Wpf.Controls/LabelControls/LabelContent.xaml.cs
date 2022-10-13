namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelContent.
/// </summary>
public partial class LabelContent : ILabelTextControl
{
    public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
        nameof(LabelText),
        typeof(string),
        typeof(LabelContent),
        new PropertyMetadata(defaultValue: string.Empty));

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public LabelContent()
    {
        InitializeComponent();
    }
}