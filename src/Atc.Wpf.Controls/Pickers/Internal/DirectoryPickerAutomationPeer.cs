namespace Atc.Wpf.Controls.Pickers.Internal;

/// <summary>Exposes <see cref="DirectoryPicker"/> to UI Automation with the value pattern.</summary>
public class DirectoryPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>Initializes a new instance of the <see cref="DirectoryPickerAutomationPeer"/> class.</summary>
    public DirectoryPickerAutomationPeer(DirectoryPicker owner)
        : base(owner)
    {
    }

    private DirectoryPicker DirectoryPicker => (DirectoryPicker)Owner;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(DirectoryPicker);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "directory picker";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value => DirectoryPicker.DisplayValue ?? string.Empty;

    /// <inheritdoc />
    public void SetValue(string value)
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        if (string.IsNullOrEmpty(value))
        {
            DirectoryPicker.SetCurrentValue(DirectoryPicker.ValueProperty, null);
            return;
        }

        DirectoryPicker.SetCurrentValue(DirectoryPicker.ValueProperty, new DirectoryInfo(value));
    }
}