// ReSharper disable CheckNamespace
namespace Atc.Wpf;

public class ChangedBooleanEventArgs : EventArgs
{
    public ChangedBooleanEventArgs(
        string identifier,
        bool? oldValue,
        bool? newValue)
    {
        Identifier = identifier;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public string Identifier { get; }

    public bool? OldValue { get; }

    public bool? NewValue { get; }

    public override string ToString()
        => $"{nameof(Identifier)}: {Identifier}, {nameof(OldValue)}: {OldValue}, {nameof(NewValue)}: {NewValue}";
}