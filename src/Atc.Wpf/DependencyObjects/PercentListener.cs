namespace Atc.Wpf.DependencyObjects;

/// <summary>
/// Message listener, singleton pattern.
/// Inherit from DependencyObject to implement DataBinding.
/// </summary>
public sealed class PercentListener : DependencyObject
{
    /// <summary>
    /// Percent Property.
    /// </summary>
    public static readonly DependencyProperty PercentProperty = DependencyProperty.Register(
        nameof(Percent),
        typeof(int),
        typeof(PercentListener),
        new UIPropertyMetadata(propertyChangedCallback: null));

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
    public static PercentListener Instance => percentListener ??= new PercentListener();

    /// <summary>
    /// Gets or sets received percent.
    /// </summary>
    public int Percent
    {
        get => (int)GetValue(PercentProperty);
        set => SetValue(PercentProperty, value);
    }

    /// <summary>
    /// Receives the percent.
    /// </summary>
    /// <param name="percent">The percent.</param>
    public void ReceivePercent(int percent)
    {
        if (percent < 0 || percent > 100)
        {
            throw new ArgumentException("Percent have to be between 0 and 100", nameof(percent));
        }

        SetCurrentValue(PercentProperty, percent);
        DispatcherHelper.DoEvents();
    }
}