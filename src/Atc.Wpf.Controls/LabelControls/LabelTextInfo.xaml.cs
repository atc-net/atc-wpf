namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelTextInfo.
/// </summary>
public partial class LabelTextInfo : ILabelTextInfo
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(LabelTextInfo),
        new PropertyMetadata(defaultValue: string.Empty));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public LabelTextInfo()
    {
        InitializeComponent();
    }
}