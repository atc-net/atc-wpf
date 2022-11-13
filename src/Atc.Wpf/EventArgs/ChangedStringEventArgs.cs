// ReSharper disable CheckNamespace
namespace Atc.Wpf;

public class ChangedStringEventArgs : EventArgs
{
    public ChangedStringEventArgs(
        string identifier,
        string? oldValue,
        string? newValue)
    {
        Identifier = identifier;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public string Identifier { get; }

    public string? OldValue { get; }

    public string? NewValue { get; }

    public override string ToString()
        => $"{nameof(Identifier)}: {Identifier}, {nameof(OldValue)}: {OldValue}, {nameof(NewValue)}: {NewValue}";
}