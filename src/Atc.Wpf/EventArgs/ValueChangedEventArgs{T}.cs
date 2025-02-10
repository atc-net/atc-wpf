// ReSharper disable CheckNamespace
// ReSharper disable ConvertToPrimaryConstructor
namespace Atc.Wpf;

public sealed class ValueChangedEventArgs<T> : EventArgs
{
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

    public string Identifier { get; }

    public T OldValue { get; }

    public T NewValue { get; }

    public override string ToString()
        => $"{nameof(Identifier)}: {Identifier}, {nameof(OldValue)}: {OldValue}, {nameof(NewValue)}: {NewValue}";
}