// ReSharper disable CheckNamespace
namespace Atc.Wpf;

public class ChangedDateTimeEventArgs : EventArgs
{
    public ChangedDateTimeEventArgs(
        string identifier,
        DateTime? oldValue,
        DateTime? newValue)
    {
        Identifier = identifier;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public string Identifier { get; }

    public DateTime? OldValue { get; }

    public DateTime? NewValue { get; }

    public override string ToString()
        => $"{nameof(Identifier)}: {Identifier}, {nameof(OldValue)}: {OldValue}, {nameof(NewValue)}: {NewValue}";
}