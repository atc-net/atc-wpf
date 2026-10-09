namespace Atc.Wpf.Controls.Inputs.Internal;

/// <summary>Exposes <see cref="NumericBox"/> to UI Automation as a spinner with a range value.</summary>
public class NumericBoxAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
{
    /// <summary>Initializes a new instance of the <see cref="NumericBoxAutomationPeer"/> class.</summary>
    public NumericBoxAutomationPeer(NumericBox owner)
        : base(owner)
    {
    }

    private NumericBox NumericBox => (NumericBox)Owner;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(NumericBox);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Spinner;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.RangeValue
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public double Value => NumericBox.Value ?? 0;

    /// <inheritdoc />
    public bool IsReadOnly => NumericBox.IsReadOnly;

    /// <inheritdoc />
    public double Maximum => NumericBox.Maximum;

    /// <inheritdoc />
    public double Minimum => NumericBox.Minimum;

    /// <inheritdoc />
    public double SmallChange => NumericBox.Interval;

    /// <inheritdoc />
    public double LargeChange => NumericBox.Interval * 10;

    /// <inheritdoc />
    public void SetValue(double value)
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        if (IsReadOnly)
        {
            throw new ElementNotEnabledException();
        }

        if (value < Minimum || value > Maximum)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        NumericBox.SetCurrentValue(
            NumericBox.ValueProperty,
            value);
    }

    [SuppressMessage("Major Code Smell", "S1244:Floating point numbers should not be tested for equality", Justification = "Nullable comparison is intentional for automation events.")]
    internal void RaiseValuePropertyChangedEvent(
        double? oldValue,
        double? newValue)
    {
        if (Nullable.Equals(oldValue, newValue))
        {
            return;
        }

        RaisePropertyChangedEvent(
            RangeValuePatternIdentifiers.ValueProperty,
            oldValue ?? 0,
            newValue ?? 0);
    }
}