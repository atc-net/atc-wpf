namespace Atc.Wpf.Mvvm;

[SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "OK.")]
public interface IViewModelBase : IObservableObject, ICleanup
{
    /// <summary>
    /// Gets or sets a value indicating whether this instance is enable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is enable; otherwise, <c>false</c>.
    /// </value>
    bool IsEnable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is visible.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
    /// </value>
    bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is busy.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
    /// </value>
    bool IsBusy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is dirty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
    /// </value>
    bool IsDirty { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [is selected].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [is selected]; otherwise, <c>false</c>.
    /// </value>
    bool IsSelected { get; set; }

    /// <summary>
    /// Broadcasts the specified old value.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    void Broadcast<T>(string propertyName, T oldValue, T newValue);

    /// <summary>
    /// Raises the property changed.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="broadcast">if set to <c>true</c> [broadcast].</param>
    /// <exception cref="ArgumentException">This method cannot be called with an empty string, propertyName</exception>
    void RaisePropertyChanged<T>(
        string propertyName,
        T oldValue = default,
        T newValue = default,
        bool broadcast = false);
}