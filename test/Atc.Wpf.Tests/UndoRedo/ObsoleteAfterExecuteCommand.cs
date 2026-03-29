namespace Atc.Wpf.Tests.UndoRedo;

/// <summary>
/// A test command that becomes obsolete after execution.
/// Used to verify that obsolete commands are discarded by the service.
/// </summary>
internal sealed class ObsoleteAfterExecuteCommand : IUndoCommand
{
    private bool executed;

    public ObsoleteAfterExecuteCommand(string description)
    {
        Description = description;
    }

    public string Description { get; }

    public bool IsObsolete => executed;

    public void Execute()
    {
        executed = true;
    }

    public void UnExecute()
    {
        executed = false;
    }
}