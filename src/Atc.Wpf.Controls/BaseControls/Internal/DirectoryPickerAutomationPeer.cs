namespace Atc.Wpf.Controls.BaseControls.Internal;

public class DirectoryPickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public DirectoryPickerAutomationPeer(DirectoryPicker owner)
        : base(owner)
    {
    }

    private DirectoryPicker DirectoryPicker => (DirectoryPicker)Owner;

    protected override string GetClassNameCore()
        => nameof(DirectoryPicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "directory picker";

    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    public bool IsReadOnly => false;

    public string Value => DirectoryPicker.DisplayValue ?? string.Empty;

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
