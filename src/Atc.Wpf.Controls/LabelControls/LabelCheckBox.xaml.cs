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
            OnIsCheckedChanged));

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public event EventHandler<ChangedBooleanEventArgs>? IsCheckedChanged;

    public LabelCheckBox()
    {
        InitializeComponent();
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnIsCheckedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelCheckBox)d;

        control.IsCheckedChanged?.Invoke(
            control,
            new ChangedBooleanEventArgs(
                control.Identifier,
                !control.IsChecked,
                control.IsChecked));
    }
}