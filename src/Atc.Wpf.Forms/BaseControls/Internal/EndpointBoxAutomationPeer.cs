namespace Atc.Wpf.Forms.BaseControls.Internal;

/// <summary>
/// Exposes an <see cref="EndpointBox"/> to UI Automation, including its endpoint URI through the Value pattern.
/// </summary>
public class EndpointBoxAutomationPeer : UserControlAutomationPeer, IValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointBoxAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The endpoint box this peer represents.</param>
    public EndpointBoxAutomationPeer(EndpointBox owner)
        : base(owner)
    {
    }

    private EndpointBox EndpointBox => (EndpointBox)Owner;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(EndpointBox);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetLocalizedControlTypeCore()
        => "endpoint";

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Value
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public string Value => EndpointBox.Value?.ToString() ?? string.Empty;

    /// <inheritdoc />
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