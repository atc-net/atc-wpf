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
            OnSelectedKeyLostFocus,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public string SelectedKey
    {
        get => (string)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    public event EventHandler<ChangedStringEventArgs>? SelectedKeyChanged;

    public LabelComboBox()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        ValidateValue(default, this, raiseEvents: false);
        return string.IsNullOrEmpty(ValidationText);
    }

    private static void OnSelectedKeyLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelComboBox)d;

        ValidateValue(e, control, raiseEvents: true);
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void ValidateValue(
        DependencyPropertyChangedEventArgs e,
        LabelComboBox control,
        bool raiseEvents)
    {
        var newValue = e.NewValue?.ToString();
        var oldValue = e.OldValue?.ToString();

        if (string.IsNullOrEmpty(newValue) &&
            string.IsNullOrEmpty(oldValue))
        {
            return;
        }

        if (control.IsMandatory &&
            string.IsNullOrWhiteSpace(control.SelectedKey) &&
            e.OldValue is not null)
        {
            control.ValidationText = Validations.FieldIsRequired;
            return;
        }

        control.ValidationText = string.Empty;

        if (raiseEvents)
        {
            control.SelectedKeyChanged?.Invoke(
                control,
                new ChangedStringEventArgs(
                    control.Identifier,
                    oldValue,
                    newValue));
        }
    }
}