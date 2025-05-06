// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls;

public sealed class TerminalReceivedDataEventArgs(string[] lines) : EventArgs
{
    public IReadOnlyList<string> Lines { get; } = lines;

    public override string ToString()
        => $"{nameof(Lines)}.Count: {Lines.Count}";
}