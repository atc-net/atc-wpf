namespace Atc.Wpf.Tests.UndoRedo;

public sealed class PropertyChangeCommandTests
{
    [Fact]
    public void Execute_CallsSetterWithNewValue()
    {
        var capturedValue = string.Empty;
        var command = new PropertyChangeCommand<string>(
            "Set Name",
            v => capturedValue = v,
            "old",
            "new");

        command.Execute();

        Assert.Equal("new", capturedValue);
    }

    [Fact]
    public void UnExecute_CallsSetterWithOldValue()
    {
        var capturedValue = string.Empty;
        var command = new PropertyChangeCommand<string>(
            "Set Name",
            v => capturedValue = v,
            "old",
            "new");

        command.UnExecute();

        Assert.Equal("old", capturedValue);
    }

    [Fact]
    public void Description_ReturnsConstructorValue()
    {
        var command = new PropertyChangeCommand<int>(
            "Change count",
            _ => { },
            0,
            42);

        Assert.Equal("Change count", command.Description);
    }

    [Fact]
    public void Constructor_NullDescription_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new PropertyChangeCommand<int>(null!, _ => { }, 0, 1));
    }

    [Fact]
    public void Constructor_NullSetter_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new PropertyChangeCommand<int>("test", null!, 0, 1));
    }

    [Fact]
    public void WorksWithNullableValues()
    {
        string? capturedValue = null;
        var command = new PropertyChangeCommand<string?>(
            "Set nullable",
            v => capturedValue = v,
            null,
            "value");

        command.Execute();
        Assert.Equal("value", capturedValue);

        command.UnExecute();
        Assert.Null(capturedValue);
    }
}