namespace Atc.Wpf.Tests.UndoRedo;

public sealed class UndoCommandTests
{
    [Fact]
    public void Constructor_NullDescription_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new UndoCommand(null!, () => { }, () => { }));
    }

    [Fact]
    public void Constructor_NullExecute_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new UndoCommand("test", null!, () => { }));
    }

    [Fact]
    public void Constructor_NullUnExecute_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new UndoCommand("test", () => { }, null!));
    }

    [Fact]
    public void Description_ReturnsConstructorValue()
    {
        var command = new UndoCommand("Set value", () => { }, () => { });
        Assert.Equal("Set value", command.Description);
    }

    [Fact]
    public void Execute_InvokesDelegate()
    {
        var called = false;
        var command = new UndoCommand("test", () => called = true, () => { });

        command.Execute();

        Assert.True(called);
    }

    [Fact]
    public void UnExecute_InvokesDelegate()
    {
        var called = false;
        var command = new UndoCommand("test", () => { }, () => called = true);

        command.UnExecute();

        Assert.True(called);
    }
}