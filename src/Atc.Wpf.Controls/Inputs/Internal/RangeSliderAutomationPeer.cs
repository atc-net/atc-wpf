namespace Atc.Wpf.Controls.Inputs.Internal;

/// <summary>
/// Automation peer for <see cref="RangeSlider"/>.
/// </summary>
/// <remarks>
/// Exposes RangeStart as the primary Value for IRangeValueProvider,
/// since the lower bound is the most common programmatic interaction point.
/// </remarks>
public class RangeSliderAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RangeSliderAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The owner control.</param>
    public RangeSliderAutomationPeer(RangeSlider owner)
        : base(owner)
    {
    }

    private RangeSlider RangeSlider => (RangeSlider)Owner;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(RangeSlider);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Slider;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.RangeValue
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public double Value => RangeSlider.RangeStart;

    /// <inheritdoc />
    public bool IsReadOnly => !IsEnabled();

    /// <inheritdoc />
    public double Maximum => RangeSlider.Maximum;

    /// <inheritdoc />
    public double Minimum => RangeSlider.Minimum;

    /// <inheritdoc />
    public double SmallChange => RangeSlider.Step;

    /// <inheritdoc />
    public double LargeChange => RangeSlider.Step * 10;

    /// <inheritdoc />
    public void SetValue(double value)
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        if (value < Minimum || value > Maximum)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        RangeSlider.SetCurrentValue(
            RangeSlider.RangeStartProperty,
            value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal void RaiseRangeStartPropertyChangedEvent(
        double oldValue,
        double newValue)
    {
        if (System.Math.Abs(oldValue - newValue) < double.Epsilon)
        {
            return;
        }

        RaisePropertyChangedEvent(
            RangeValuePatternIdentifiers.ValueProperty,
            oldValue,
            newValue);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal void RaiseRangeEndPropertyChangedEvent(
        double oldValue,
        double newValue)
    {
        if (System.Math.Abs(oldValue - newValue) < double.Epsilon)
        {
            return;
        }

        RaisePropertyChangedEvent(
            RangeValuePatternIdentifiers.ValueProperty,
            oldValue,
            newValue);
    }
}