namespace Atc.Wpf.Controls.Inputs.Internal;

/// <summary>
/// Automation peer for <see cref="Rating"/>.
/// </summary>
public class RatingAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RatingAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The owner control.</param>
    public RatingAutomationPeer(Rating owner)
        : base(owner)
    {
    }

    private Rating Rating => (Rating)Owner;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => "Rating";

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Slider;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.RangeValue
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public double Value => Rating.Value;

    /// <inheritdoc />
    public bool IsReadOnly => Rating.IsReadOnly;

    /// <inheritdoc />
    public double Maximum => Rating.Maximum;

    /// <inheritdoc />
    public double Minimum => 0;

    /// <inheritdoc />
    public double LargeChange => 1;

    /// <inheritdoc />
    public double SmallChange => Rating.AllowHalfStars ? 0.5 : 1;

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

        Rating.SetCurrentValue(Rating.ValueProperty, value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal void RaiseValuePropertyChangedEvent(
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