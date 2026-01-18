namespace Atc.Wpf.Forms.BaseControls.Internal;

public class EndpointBoxAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    public EndpointBoxAutomationPeer(EndpointBox owner)
        : base(owner)
    {
    }

    private EndpointBox EndpointBox => (EndpointBox)Owner;

    protected override string GetClassNameCore()
        => nameof(EndpointBox);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetLocalizedControlTypeCore()
        => "endpoint";

    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    public bool IsReadOnly => false;

    public string Value => EndpointBox.Value?.ToString() ?? string.Empty;

    public void SetValue(string value)
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        if (string.IsNullOrEmpty(value))
        {
            EndpointBox.SetCurrentValue(EndpointBox.ValueProperty, null);
            return;
        }

        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            EndpointBox.SetCurrentValue(EndpointBox.ValueProperty, uri);
        }
        else
        {
            throw new ArgumentException($"Invalid URI value: {value}", nameof(value));
        }
    }
}