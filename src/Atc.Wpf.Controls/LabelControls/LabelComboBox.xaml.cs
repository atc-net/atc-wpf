namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelComboBox.
/// </summary>
public partial class LabelComboBox : ILabelComboBox
{
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        nameof(Items),
        typeof(Dictionary<string, string>),
        typeof(LabelComboBox),
        new PropertyMetadata(default(Dictionary<string, string>)));

    public Dictionary<string, string> Items
    {
        get => (Dictionary<string, string>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
        nameof(SelectedKey),
        typeof(string),
        typeof(LabelComboBox),
        new FrameworkPropertyMetadata(
            default(string),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnSelectedKeyChanged));

    public string SelectedKey
    {
        get => (string)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    public event EventHandler<SelectedKeyEventArgs>? SelectedKeyChanged;

    public LabelComboBox()
    {
        InitializeComponent();
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnSelectedKeyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var labelComboBox = (LabelComboBox)d;
        var newValue = e.NewValue?.ToString();
        var oldValue = e.OldValue?.ToString();

        if (string.IsNullOrEmpty(newValue) &&
            string.IsNullOrEmpty(oldValue))
        {
            return;
        }

        if (labelComboBox.IsMandatory &&
            string.IsNullOrWhiteSpace(labelComboBox.SelectedKey) &&
            e.OldValue is not null)
        {
            labelComboBox.ValidationText = "Field is required";
            return;
        }

        labelComboBox.ValidationText = string.Empty;

        var identifier = labelComboBox.Tag is null
            ? labelComboBox.LabelText
            : labelComboBox.Tag.ToString();

        labelComboBox.SelectedKeyChanged?.Invoke(
            labelComboBox,
            new SelectedKeyEventArgs(
                identifier!,
                newValue,
                oldValue));
    }
}