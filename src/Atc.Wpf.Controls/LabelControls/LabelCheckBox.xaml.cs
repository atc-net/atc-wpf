namespace Atc.Wpf.Controls.LabelControls;

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