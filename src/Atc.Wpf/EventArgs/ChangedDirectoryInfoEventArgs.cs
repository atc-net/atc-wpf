// ReSharper disable CheckNamespace
namespace Atc.Wpf;

public class ChangedDirectoryInfoEventArgs : EventArgs
{
    public ChangedDirectoryInfoEventArgs(
        string identifier,
        DirectoryInfo? oldValue,
        DirectoryInfo? newValue)
    {
        Identifier = identifier;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public string Identifier { get; }

    public DirectoryInfo? OldValue { get; }

    public DirectoryInfo? NewValue { get; }

    public override string ToString()
        => $"{nameof(Identifier)}: {Identifier}, {nameof(OldValue)}: {OldValue}, {nameof(NewValue)}: {NewValue}";
}