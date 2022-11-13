// ReSharper disable CheckNamespace
namespace Atc.Wpf;

public class ChangedDecimalEventArgs : EventArgs
{
    public ChangedDecimalEventArgs(
        string identifier,
        decimal? oldValue,
        decimal? newValue)
    {
        Identifier = identifier;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public string Identifier { get; }

    public decimal? OldValue { get; }

    public decimal? NewValue { get; }

    public override string ToString()
        => $"{nameof(Identifier)}: {Identifier}, {nameof(OldValue)}: {OldValue}, {nameof(NewValue)}: {NewValue}";
}