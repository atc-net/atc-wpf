namespace Atc.Wpf.Tests.UndoRedo;

internal sealed class MergeableTestCommand : IMergeableUndoCommand
{
    private int value;

    public MergeableTestCommand(
        int mergeId,
        int value)
    {
        MergeId = mergeId;
        this.value = value;
    }

    public string Description => $"Set to {value}";

    public int MergeId { get; }

    public int Value => value;

    public bool TryMergeWith(IUndoCommand other)
    {
        if (other is MergeableTestCommand m && m.MergeId == MergeId)
        {
            value = m.value;
            return true;
        }

        return false;
    }

    public void Execute()
    {
    }

    public void UnExecute()
    {
    }
}