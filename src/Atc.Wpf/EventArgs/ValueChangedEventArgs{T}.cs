// ReSharper disable CheckNamespace
// ReSharper disable ConvertToPrimaryConstructor
namespace Atc.Wpf;

/// <summary>
/// Provides data for an event raised when a value changes.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public sealed class ValueChangedEventArgs<T> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueChangedEventArgs{T}"/> class.
    /// </summary>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "OK.")]
    public ValueChangedEventArgs(
        string identifier,
        T oldValue,
        T newValue)
    {
        Identifier = identifier;
        OldValue = oldValue;
        NewValue = newValue;
    }

    /// <summary>
    /// Gets the identifier of the value that changed.
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// Gets the old value.
    /// </summary>
    public T OldValue { get; }

    /// <summary>
    /// Gets the new value.
    /// </summary>
    public T NewValue { get; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Identifier)}: {Identifier}, {nameof(OldValue)}: {OldValue}, {nameof(NewValue)}: {NewValue}";
}