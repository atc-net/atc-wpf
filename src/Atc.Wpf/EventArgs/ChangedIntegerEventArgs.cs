// ReSharper disable CheckNamespace
namespace Atc.Wpf;

public class ChangedIntegerEventArgs : EventArgs
{
    public ChangedIntegerEventArgs(
        string identifier,
        int? oldValue,
        int? newValue)
    {
        Identifier = identifier;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public string Identifier { get; }

    public int? OldValue { get; }

    public int? NewValue { get; }

    public override string ToString()
        => $"{nameof(Identifier)}: {Identifier}, {nameof(OldValue)}: {OldValue}, {nameof(NewValue)}: {NewValue}";
}