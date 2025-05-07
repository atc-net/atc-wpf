namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelComboBox : ILabelComboBox
{
    [DependencyProperty]
    private Dictionary<string, string> items;

    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnSelectedKeyLostFocus),
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private string selectedKey;

    public event EventHandler<ValueChangedEventArgs<string?>>? SelectorChanged;

    public event EventHandler<ValueChangedEventArgs<string?>>? SelectorLostFocusInvalid;

    public LabelComboBox()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        ValidateValue(default, this, SelectedKey, raiseEvents: false);
        return string.IsNullOrEmpty(ValidationText);
    }

    private static void ValidateValue(
        DependencyPropertyChangedEventArgs e,
        LabelComboBox control,
        string? selectedKey,
        bool raiseEvents)
    {
        if (control.IsMandatory &&
            string.IsNullOrWhiteSpace(selectedKey))
        {
            control.ValidationText = Validations.FieldIsRequired;
            if (raiseEvents)
            {
                OnSelectorLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        control.ValidationText = string.Empty;

        if (!raiseEvents)
        {
            return;
        }

        control.SelectorChanged?.Invoke(
            control,
            new ValueChangedEventArgs<string?>(
                control.Identifier,
                e.OldValue?.ToString(),
                selectedKey));
    }

    private static void OnSelectedKeyLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelComboBox)d;

        ValidateValue(e, control, control.SelectedKey, raiseEvents: true);
    }

    private void OnSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1)
        {
            return;
        }

        var newValue = e.AddedItems[0]!.ToString()!;

        Debug.WriteLine($"LabelComboBox - Change to: {newValue}");
        ValidateValue(default, this, newValue, raiseEvents: false);
    }

    private static void OnSelectorLostFocusFireInvalidEvent(
        LabelComboBox control,
        DependencyPropertyChangedEventArgs e)
    {
        var oldValue = e.OldValue is null
            ? string.Empty
            : e.OldValue.ToString();

        var newValue = e.NewValue is null
            ? string.Empty
            : e.NewValue.ToString();

        control.SelectorLostFocusInvalid?.Invoke(
            control,
            new ValueChangedEventArgs<string?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }
}