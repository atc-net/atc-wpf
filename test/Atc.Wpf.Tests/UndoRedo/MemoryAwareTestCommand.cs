namespace Atc.Wpf.Tests.UndoRedo;

internal sealed class MemoryAwareTestCommand(
    string description,
    long estimatedMemoryBytes)
    : IMemoryAwareUndoCommand
{
    public string Description { get; } = description;

    public long EstimatedMemoryBytes { get; } = estimatedMemoryBytes;

    public void Execute()
    {
    }

    public void UnExecute()
    {
    }
}