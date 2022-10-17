namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelCheckBox.
/// </summary>
public partial class LabelCheckBox : ILabelCheckBox
{
    public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
        nameof(IsChecked),
        typeof(bool),
        typeof(LabelCheckBox),
        new FrameworkPropertyMetadata(
            defaultValue: false,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            propertyChangedCallback: null));

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public LabelCheckBox()
    {
        InitializeComponent();
    }
}