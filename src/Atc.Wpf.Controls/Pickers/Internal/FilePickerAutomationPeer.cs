namespace Atc.Wpf.Controls.Pickers.Internal;

public class FilePickerAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public FilePickerAutomationPeer(FilePicker owner)
        : base(owner)
    {
    }

    private FilePicker FilePicker => (FilePicker)Owner;

    protected override string GetClassNameCore()
        => nameof(FilePicker);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "file picker";

    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    public bool IsReadOnly => false;

    public string Value => FilePicker.DisplayValue ?? string.Empty;

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