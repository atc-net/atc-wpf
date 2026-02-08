namespace Atc.Wpf.Tests.UndoRedo;

public sealed class UndoRedoServiceTests
{
    private readonly UndoRedoService sut = new();

    [Fact]
    public void Execute_PushesCommandToUndoStack()
    {
        var value = 0;
        var command = new UndoCommand("inc", () => value++, () => value--);

        sut.Execute(command);

        Assert.Equal(1, value);
        Assert.Single(sut.UndoCommands);
        Assert.Same(command, sut.UndoCommands[0]);
        Assert.True(sut.CanUndo);
    }

    [Fact]
    public void Execute_ClearsRedoStack()
    {
        var value = 0;
        sut.Execute(new UndoCommand("inc", () => value++, () => value--));
        sut.Undo();
        Assert.True(sut.CanRedo);

        sut.Execute(new UndoCommand("set", () => value = 10, () => value = 0));

        Assert.False(sut.CanRedo);
        Assert.Empty(sut.RedoCommands);
    }

    [Fact]
    public void Add_PushesWithoutExecuting()
    {
        var executed = false;
        var command = new UndoCommand("test", () => executed = true, () => { });

        sut.Add(command);

        Assert.False(executed);
        Assert.Single(sut.UndoCommands);
    }

    [Fact]
    public void Add_ClearsRedoStack()
    {
        var value = 0;
        sut.Execute(new UndoCommand("inc", () => value++, () => value--));
        sut.Undo();
        Assert.True(sut.CanRedo);

        sut.Add(new UndoCommand("other", () => { }, () => { }));

        Assert.False(sut.CanRedo);
    }

    [Fact]
    public void Undo_RevertsLastCommand()
    {
        var value = 0;
        sut.Execute(new UndoCommand("inc", () => value++, () => value--));
        Assert.Equal(1, value);

        var result = sut.Undo();

        Assert.True(result);
        Assert.Equal(0, value);
        Assert.Empty(sut.UndoCommands);
        Assert.Single(sut.RedoCommands);
    }

    [Fact]
    public void Undo_ReturnsFalseWhenEmpty()
    {
        var result = sut.Undo();
        Assert.False(result);
    }

    [Fact]
    public void Redo_ReExecutesCommand()
    {
        var value = 0;
        sut.Execute(new UndoCommand("inc", () => value++, () => value--));
        sut.Undo();
        Assert.Equal(0, value);

        var result = sut.Redo();

        Assert.True(result);
        Assert.Equal(1, value);
        Assert.Single(sut.UndoCommands);
        Assert.Empty(sut.RedoCommands);
    }

    [Fact]
    public void Redo_ReturnsFalseWhenEmpty()
    {
        var result = sut.Redo();
        Assert.False(result);
    }

    [Fact]
    public void UndoAll_RevertsAllCommands()
    {
        var value = 0;
        sut.Execute(new UndoCommand("a", () => value++, () => value--));
        sut.Execute(new UndoCommand("b", () => value++, () => value--));
        sut.Execute(new UndoCommand("c", () => value++, () => value--));
        Assert.Equal(3, value);

        sut.UndoAll();

        Assert.Equal(0, value);
        Assert.Empty(sut.UndoCommands);
        Assert.Equal(3, sut.RedoCommands.Count);
    }

    [Fact]
    public void RedoAll_RedoesAllCommands()
    {
        var value = 0;
        sut.Execute(new UndoCommand("a", () => value++, () => value--));
        sut.Execute(new UndoCommand("b", () => value++, () => value--));
        sut.Execute(new UndoCommand("c", () => value++, () => value--));
        sut.UndoAll();
        Assert.Equal(0, value);

        sut.RedoAll();

        Assert.Equal(3, value);
        Assert.Equal(3, sut.UndoCommands.Count);
        Assert.Empty(sut.RedoCommands);
    }

    [Fact]
    public void UndoTo_UndoesUpToSpecifiedCommand()
    {
        var value = 0;
        var cmdA = new UndoCommand("a", () => value++, () => value--);
        var cmdB = new UndoCommand("b", () => value++, () => value--);
        var cmdC = new UndoCommand("c", () => value++, () => value--);
        sut.Execute(cmdA);
        sut.Execute(cmdB);
        sut.Execute(cmdC);
        Assert.Equal(3, value);

        sut.UndoTo(cmdB);

        Assert.Equal(1, value);
        Assert.Single(sut.UndoCommands);
        Assert.Same(cmdA, sut.UndoCommands[0]);
    }

    [Fact]
    public void RedoTo_RedoesUpToSpecifiedCommand()
    {
        var value = 0;
        var cmdA = new UndoCommand("a", () => value++, () => value--);
        var cmdB = new UndoCommand("b", () => value++, () => value--);
        var cmdC = new UndoCommand("c", () => value++, () => value--);
        sut.Execute(cmdA);
        sut.Execute(cmdB);
        sut.Execute(cmdC);
        sut.UndoAll();

        sut.RedoTo(cmdB);

        Assert.Equal(2, value);
        Assert.Equal(2, sut.UndoCommands.Count);
        Assert.Single(sut.RedoCommands);
    }

    [Fact]
    public void BeginGroup_CollectsCommandsAsOneUnit()
    {
        var value = 0;

        using (sut.BeginGroup("group op"))
        {
            sut.Execute(new UndoCommand("a", () => value++, () => value--));
            sut.Execute(new UndoCommand("b", () => value++, () => value--));
        }

        Assert.Equal(2, value);
        Assert.Single(sut.UndoCommands);
        Assert.Equal("group op", sut.UndoCommands[0].Description);
    }

    [Fact]
    public void BeginGroup_UndoRevertsEntireGroup()
    {
        var value = 0;

        using (sut.BeginGroup("group"))
        {
            sut.Execute(new UndoCommand("a", () => value++, () => value--));
            sut.Execute(new UndoCommand("b", () => value++, () => value--));
        }

        Assert.Equal(2, value);

        sut.Undo();

        Assert.Equal(0, value);
    }

    [Fact]
    public void BeginGroup_EmptyGroupDoesNotPush()
    {
        using (sut.BeginGroup("empty"))
        {
            // No commands added.
        }

        Assert.Empty(sut.UndoCommands);
    }

    [Fact]
    public void BeginGroup_NestedGroupThrows()
    {
        using (sut.BeginGroup("outer"))
        {
            Assert.Throws<InvalidOperationException>(
                () => sut.BeginGroup("inner"));
        }
    }

    [Fact]
    public void BeginGroup_AddWithoutExecute_CollectsInGroup()
    {
        var executed = false;

        using (sut.BeginGroup("group"))
        {
            sut.Add(new UndoCommand("a", () => executed = true, () => { }));
        }

        Assert.False(executed);
        Assert.Single(sut.UndoCommands);
    }

    [Fact]
    public void MaxHistorySize_TrimsOldestCommands()
    {
        sut.MaxHistorySize = 3;

        for (var i = 0; i < 5; i++)
        {
            var capture = i;
            sut.Execute(new UndoCommand($"cmd{capture}", () => { }, () => { }));
        }

        Assert.Equal(3, sut.UndoCommands.Count);
        Assert.Equal("cmd4", sut.UndoCommands[0].Description);
        Assert.Equal("cmd3", sut.UndoCommands[1].Description);
        Assert.Equal("cmd2", sut.UndoCommands[2].Description);
    }

    [Fact]
    public void IsExecuting_GuardsAgainstRecursion()
    {
        var service = sut;

        var innerAdded = false;
        var command = new UndoCommand(
            "recursive",
            () => service.Add(new UndoCommand("inner", () => innerAdded = true, () => { })),
            () => { });

        service.Execute(command);

        Assert.False(innerAdded);
        Assert.Single(service.UndoCommands);
    }

    [Fact]
    public void Clear_RemovesBothStacks()
    {
        var value = 0;
        sut.Execute(new UndoCommand("a", () => value++, () => value--));
        sut.Execute(new UndoCommand("b", () => value++, () => value--));
        sut.Undo();

        sut.Clear();

        Assert.Empty(sut.UndoCommands);
        Assert.Empty(sut.RedoCommands);
        Assert.False(sut.CanUndo);
        Assert.False(sut.CanRedo);
    }

    [Fact]
    public void ActionPerformed_FiredOnExecute()
    {
        UndoRedoEventArgs? receivedArgs = null;
        sut.ActionPerformed += (_, e) => receivedArgs = e;

        var command = new UndoCommand("test", () => { }, () => { });
        sut.Execute(command);

        Assert.NotNull(receivedArgs);
        Assert.Equal(UndoRedoActionType.Execute, receivedArgs.ActionType);
        Assert.Same(command, receivedArgs.Command);
    }

    [Fact]
    public void ActionPerformed_FiredOnUndo()
    {
        UndoRedoEventArgs? receivedArgs = null;
        var command = new UndoCommand("test", () => { }, () => { });
        sut.Execute(command);

        sut.ActionPerformed += (_, e) => receivedArgs = e;
        sut.Undo();

        Assert.NotNull(receivedArgs);
        Assert.Equal(UndoRedoActionType.Undo, receivedArgs.ActionType);
    }

    [Fact]
    public void ActionPerformed_FiredOnRedo()
    {
        UndoRedoEventArgs? receivedArgs = null;
        sut.Execute(new UndoCommand("test", () => { }, () => { }));
        sut.Undo();

        sut.ActionPerformed += (_, e) => receivedArgs = e;
        sut.Redo();

        Assert.NotNull(receivedArgs);
        Assert.Equal(UndoRedoActionType.Redo, receivedArgs.ActionType);
    }

    [Fact]
    public void StateChanged_FiredOnExecute()
    {
        var fired = false;
        sut.StateChanged += (_, _) => fired = true;

        sut.Execute(new UndoCommand("test", () => { }, () => { }));

        Assert.True(fired);
    }

    [Fact]
    public void StateChanged_FiredOnClear()
    {
        sut.Execute(new UndoCommand("test", () => { }, () => { }));

        var fired = false;
        sut.StateChanged += (_, _) => fired = true;

        sut.Clear();

        Assert.True(fired);
    }

    [Fact]
    public void Execute_NullCommand_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => sut.Execute(null!));
    }

    [Fact]
    public void Add_NullCommand_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => sut.Add(null!));
    }

    [Fact]
    public void ActionPerformed_FiredOnAdd()
    {
        UndoRedoEventArgs? receivedArgs = null;
        sut.ActionPerformed += (_, e) => receivedArgs = e;

        var command = new UndoCommand("test", () => { }, () => { });
        sut.Add(command);

        Assert.NotNull(receivedArgs);
        Assert.Equal(UndoRedoActionType.Execute, receivedArgs.ActionType);
        Assert.Same(command, receivedArgs.Command);
    }
}