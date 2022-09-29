// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

public class SelectedKeyEventArgs : EventArgs
{
    public SelectedKeyEventArgs(
        string identifier,
        string? newKey,
        string? oldKey)
    {
        Identifier = identifier;
        NewKey = newKey;
        OldKey = oldKey;
    }

    public string Identifier { get; }

    public string? NewKey { get; }

    public string? OldKey { get; }

    public override string ToString()
        => $"{nameof(NewKey)}: {NewKey}, {nameof(OldKey)}: {OldKey}";
}