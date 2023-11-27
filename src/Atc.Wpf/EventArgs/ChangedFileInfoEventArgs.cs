// ReSharper disable CheckNamespace
namespace Atc.Wpf;

public class ChangedFileInfoEventArgs : EventArgs
{
    public ChangedFileInfoEventArgs(
        string identifier,
        FileInfo? oldValue,
        FileInfo? newValue)
    {
        Identifier = identifier;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public string Identifier { get; }

    public FileInfo? OldValue { get; }

    public FileInfo? NewValue { get; }

    public override string ToString()
        => $"{nameof(Identifier)}: {Identifier}, {nameof(OldValue)}: {OldValue}, {nameof(NewValue)}: {NewValue}";
}