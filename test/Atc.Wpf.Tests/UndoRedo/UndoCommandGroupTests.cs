namespace Atc.Wpf.Tests.UndoRedo;

public sealed class UndoCommandGroupTests
{
    [Fact]
    public void Execute_RunsCommandsInOrder()
    {
        var order = new List<int>();
        var commands = new List<IUndoCommand>
        {
            new UndoCommand("A", () => order.Add(1), () => { }),
            new UndoCommand("B", () => order.Add(2), () => { }),
            new UndoCommand("C", () => order.Add(3), () => { }),
        };

        var group = new UndoCommandGroup("group", commands);

        group.Execute();

        Assert.Equal([1, 2, 3], order);
    }

    [Fact]
    public void UnExecute_RunsCommandsInReverseOrder()
    {
        var order = new List<int>();
        var commands = new List<IUndoCommand>
        {
            new UndoCommand("A", () => { }, () => order.Add(1)),
            new UndoCommand("B", () => { }, () => order.Add(2)),
            new UndoCommand("C", () => { }, () => order.Add(3)),
        };

        var group = new UndoCommandGroup("group", commands);

        group.UnExecute();

        Assert.Equal([3, 2, 1], order);
    }

    [Fact]
    public void EmptyGroup_DoesNotThrow()
    {
        var group = new UndoCommandGroup("empty", []);

        var exception = Record.Exception(() =>
        {
            group.Execute();
            group.UnExecute();
        });

        Assert.Null(exception);
    }

    [Fact]
    public void Description_ReturnsConstructorValue()
    {
        var group = new UndoCommandGroup("Move objects", []);
        Assert.Equal("Move objects", group.Description);
    }
}