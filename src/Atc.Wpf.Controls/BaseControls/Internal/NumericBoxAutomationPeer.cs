namespace Atc.Wpf.Controls.BaseControls.Internal;

public class NumericBoxAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
{
    public NumericBoxAutomationPeer(NumericBox owner)
        : base(owner)
    {
    }

    private NumericBox NumericBox => (NumericBox)Owner;

    protected override string GetClassNameCore()
        => nameof(NumericBox);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Spinner;

    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.RangeValue
            ? this
            : base.GetPattern(patternInterface);

    public double Value => NumericBox.Value ?? 0;

    public bool IsReadOnly => NumericBox.IsReadOnly;

    public double Maximum => NumericBox.Maximum;

    public double Minimum => NumericBox.Minimum;

    public double SmallChange => NumericBox.Interval;

    public double LargeChange => NumericBox.Interval * 10;

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