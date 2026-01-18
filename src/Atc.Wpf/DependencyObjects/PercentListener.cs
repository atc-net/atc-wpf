namespace Atc.Wpf.DependencyObjects;

/// <summary>
/// Message listener, singleton pattern.
/// Inherit from DependencyObject to implement DataBinding.
/// </summary>
public sealed class PercentListener : DependencyObject
{
    public static readonly DependencyProperty PercentProperty = DependencyProperty.Register(
        nameof(Percent),
        typeof(double),
        typeof(PercentListener),
        new PropertyMetadata(default(double)));

    public double Percent
    {
        get => (double)GetValue(PercentProperty);
        set => SetValue(PercentProperty, value);
    }

    private static PercentListener? percentListener;

    /// <summary>
    /// Prevents a default instance of the <see cref="PercentListener" /> class from being created.
    /// </summary>
    private PercentListener()
    {
    }

    /// <summary>
    /// Gets MessageListener instance.
    /// </summary>
    public static PercentListener Instance
        => percentListener ??= new PercentListener();

    /// <summary>
    /// Receives the percent.
    /// </summary>
    /// <param name="percentValue">The percent value.</param>
    public void ReceivePercent(double percentValue)
    {
        if (percentValue is < 0 or > 100)
        {
            throw new ArgumentException("Percent have to be between 0 and 100", nameof(percentValue));
        }

        SetCurrentValue(PercentProperty, percentValue);
        DispatcherHelper.DoEvents();
    }
}