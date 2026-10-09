namespace Atc.Wpf.Controls.Pickers.Internal;

/// <summary>Exposes <see cref="FilePicker"/> to UI Automation with the value pattern.</summary>
public class FilePickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>Initializes a new instance of the <see cref="FilePickerAutomationPeer"/> class.</summary>
    public FilePickerAutomationPeer(FilePicker owner)
        : base(owner)
    {
    }

    private FilePicker FilePicker => (FilePicker)Owner;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(FilePicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "file picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value => FilePicker.DisplayValue ?? string.Empty;

    /// <inheritdoc />
    public void SetValue(string value)
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        if (string.IsNullOrEmpty(value))
        {
            FilePicker.SetCurrentValue(FilePicker.ValueProperty, null);
            return;
        }

        FilePicker.SetCurrentValue(FilePicker.ValueProperty, new FileInfo(value));
    }
}