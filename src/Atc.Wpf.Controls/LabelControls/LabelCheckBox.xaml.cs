namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelCheckBox : ILabelCheckBox
{
    [DependencyProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnIsCheckedChanged))]
    private bool isChecked;

    public event EventHandler<ValueChangedEventArgs<bool>>? IsCheckedChanged;

    public LabelCheckBox()
    {
        InitializeComponent();
    }

    private static void OnIsCheckedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelCheckBox)d;

        control.IsCheckedChanged?.Invoke(
            control,
            new ValueChangedEventArgs<bool>(
                control.Identifier,
                !control.IsChecked,
                control.IsChecked));
    }
}