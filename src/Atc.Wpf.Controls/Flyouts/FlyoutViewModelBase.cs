namespace Atc.Wpf.Controls.Flyouts;

/// <summary>
/// Base class for ViewModels that are displayed in flyouts.
/// Provides common functionality for closing the flyout with a result.
/// </summary>
public abstract class FlyoutViewModelBase : INotifyPropertyChanged
{
    private readonly IFlyoutService? flyoutService;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutViewModelBase"/> class.
    /// </summary>
    protected FlyoutViewModelBase()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutViewModelBase"/> class.
    /// </summary>
    /// <param name="flyoutService">The flyout service for closing the flyout.</param>
    protected FlyoutViewModelBase(IFlyoutService flyoutService)
    {
        this.flyoutService = flyoutService;
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the result value to return when the flyout closes.
    /// </summary>
    public object? Result { get; protected set; }

    /// <summary>
    /// Closes the flyout without a result.
    /// </summary>
    protected virtual void Close()
    {
        flyoutService?.CloseTopFlyout();
    }

    /// <summary>
    /// Closes the flyout with the specified result.
    /// </summary>
    /// <param name="result">The result value.</param>
    protected virtual void CloseWithResult(object? result)
    {
        Result = result;
        flyoutService?.CloseTopFlyoutWithResult(result);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets a field value and raises PropertyChanged if the value changed.
    /// </summary>
    /// <typeparam name="T">The type of the field.</typeparam>
    /// <param name="field">The field to set.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>True if the value changed; otherwise, false.</returns>
    protected bool SetProperty<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}