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
    public void HasUnsavedChanges_FalseInitially()
    {
        Assert.False(sut.HasUnsavedChanges);
    }

    [Fact]
    public void HasUnsavedChanges_TrueAfterExecute()
    {
        sut.Execute(new UndoCommand("test", () => { }, () => { }));

        Assert.True(sut.HasUnsavedChanges);
    }

    [Fact]
    public void MarkSaved_ClearsHasUnsavedChanges()
    {
        sut.Execute(new UndoCommand("test", () => { }, () => { }));
        Assert.True(sut.HasUnsavedChanges);

        sut.MarkSaved();

        Assert.False(sut.HasUnsavedChanges);
    }

    [Fact]
    public void HasUnsavedChanges_TrueAfterExecutePostSave()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        sut.MarkSaved();

        sut.Execute(new UndoCommand("b", () => { }, () => { }));

        Assert.True(sut.HasUnsavedChanges);
    }

    [Fact]
    public void HasUnsavedChanges_FalseAfterUndoBackToSavePoint()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        sut.MarkSaved();
        sut.Execute(new UndoCommand("b", () => { }, () => { }));
        Assert.True(sut.HasUnsavedChanges);

        sut.Undo();

        Assert.False(sut.HasUnsavedChanges);
    }

    [Fact]
    public void HasUnsavedChanges_TrueAfterUndoPastSavePoint()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        sut.MarkSaved();

        sut.Undo();

        Assert.True(sut.HasUnsavedChanges);
    }

    [Fact]
    public void HasUnsavedChanges_FalseAfterRedoBackToSavePoint()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        sut.MarkSaved();
        sut.Undo();
        Assert.True(sut.HasUnsavedChanges);

        sut.Redo();

        Assert.False(sut.HasUnsavedChanges);
    }

    [Fact]
    public void HasUnsavedChanges_FalseAfterClear()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        sut.MarkSaved();
        sut.Execute(new UndoCommand("b", () => { }, () => { }));

        sut.Clear();

        Assert.False(sut.HasUnsavedChanges);
    }

    [Fact]
    public void HasUnsavedChanges_TrueWhenSavePointTrimmed()
    {
        sut.MaxHistorySize = 2;
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        sut.MarkSaved();
        sut.Execute(new UndoCommand("b", () => { }, () => { }));
        sut.Execute(new UndoCommand("c", () => { }, () => { }));

        // "a" (the saved command) should have been trimmed
        Assert.True(sut.HasUnsavedChanges);

        // Undoing back won't help since save point is gone
        sut.UndoAll();
        Assert.True(sut.HasUnsavedChanges);
    }

    [Fact]
    public void MarkSaved_AtInitialState_TracksCorrectly()
    {
        // Mark saved when no commands exist
        sut.MarkSaved();
        Assert.False(sut.HasUnsavedChanges);

        // Execute makes it dirty
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        Assert.True(sut.HasUnsavedChanges);

        // Undo back to initial state makes it clean
        sut.Undo();
        Assert.False(sut.HasUnsavedChanges);
    }

    [Fact]
    public void MarkSaved_FiresStateChanged()
    {
        var fired = false;
        sut.StateChanged += (_, _) => fired = true;

        sut.MarkSaved();

        Assert.True(fired);
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

    [Fact]
    public void Undo_ByLevels_UndoesCorrectNumber()
    {
        var value = 0;
        for (var i = 0; i < 5; i++)
        {
            sut.Execute(new UndoCommand($"cmd{i}", () => value++, () => value--));
        }

        Assert.Equal(5, value);

        var result = sut.Undo(3);

        Assert.True(result);
        Assert.Equal(2, value);
        Assert.Equal(2, sut.UndoCommands.Count);
        Assert.Equal(3, sut.RedoCommands.Count);
    }

    [Fact]
    public void Undo_ByLevels_StopsWhenStackEmpty()
    {
        var value = 0;
        sut.Execute(new UndoCommand("a", () => value++, () => value--));
        sut.Execute(new UndoCommand("b", () => value++, () => value--));

        var result = sut.Undo(5);

        Assert.True(result);
        Assert.Equal(0, value);
        Assert.Empty(sut.UndoCommands);
    }

    [Fact]
    public void Undo_ByLevels_ZeroThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => sut.Undo(0));
    }

    [Fact]
    public void Undo_ByLevels_NegativeThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => sut.Undo(-1));
    }

    [Fact]
    public void Undo_ByLevels_ReturnsFalseWhenEmpty()
    {
        var result = sut.Undo(3);

        Assert.False(result);
    }

    [Fact]
    public void Redo_ByLevels_RedoesCorrectNumber()
    {
        var value = 0;
        for (var i = 0; i < 5; i++)
        {
            sut.Execute(new UndoCommand($"cmd{i}", () => value++, () => value--));
        }

        sut.UndoAll();
        Assert.Equal(0, value);

        var result = sut.Redo(3);

        Assert.True(result);
        Assert.Equal(3, value);
        Assert.Equal(3, sut.UndoCommands.Count);
        Assert.Equal(2, sut.RedoCommands.Count);
    }

    [Fact]
    public void Redo_ByLevels_StopsWhenStackEmpty()
    {
        var value = 0;
        sut.Execute(new UndoCommand("a", () => value++, () => value--));
        sut.Execute(new UndoCommand("b", () => value++, () => value--));
        sut.UndoAll();

        var result = sut.Redo(5);

        Assert.True(result);
        Assert.Equal(2, value);
        Assert.Empty(sut.RedoCommands);
    }

    [Fact]
    public void Redo_ByLevels_ZeroThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => sut.Redo(0));
    }

    [Fact]
    public void Redo_ByLevels_ReturnsFalseWhenEmpty()
    {
        var result = sut.Redo(3);

        Assert.False(result);
    }

    [Fact]
    public void ExecutingAction_IsNone_WhenIdle()
    {
        Assert.Equal(UndoRedoActionType.None, sut.ExecutingAction);
    }

    [Fact]
    public void ExecutingCommand_IsNull_WhenIdle()
    {
        Assert.Null(sut.ExecutingCommand);
    }

    [Fact]
    public void ExecutingAction_DuringExecute_IsExecute()
    {
        UndoRedoActionType? captured = null;
        var command = new UndoCommand("test", () => captured = sut.ExecutingAction, () => { });

        sut.Execute(command);

        Assert.Equal(UndoRedoActionType.Execute, captured);
    }

    [Fact]
    public void ExecutingCommand_DuringExecute_IsTheCommand()
    {
        IUndoCommand? captured = null;
        var command = new UndoCommand("test", () => captured = sut.ExecutingCommand, () => { });

        sut.Execute(command);

        Assert.Same(command, captured);
    }

    [Fact]
    public void ExecutingAction_DuringUndo_IsUndo()
    {
        UndoRedoActionType? captured = null;
        var command = new UndoCommand("test", () => { }, () => captured = sut.ExecutingAction);
        sut.Execute(command);

        sut.Undo();

        Assert.Equal(UndoRedoActionType.Undo, captured);
    }

    [Fact]
    public void ExecutingAction_DuringRedo_IsRedo()
    {
        UndoRedoActionType? captured = null;
        var command = new UndoCommand("test", () => captured = sut.ExecutingAction, () => { });
        sut.Execute(command);
        captured = null;
        sut.Undo();

        sut.Redo();

        Assert.Equal(UndoRedoActionType.Redo, captured);
    }

    [Fact]
    public void ExecutingAction_AfterComplete_IsNone()
    {
        sut.Execute(new UndoCommand("test", () => { }, () => { }));

        Assert.Equal(UndoRedoActionType.None, sut.ExecutingAction);
        Assert.Null(sut.ExecutingCommand);
    }

    [Fact]
    public void UndoToLastUserAction_UndoesUserActionAndTrailingInternals()
    {
        // Stack: [internal2, internal1, user1] (top to bottom)
        // P1: skip (top is non-user) → undo internal2, internal1
        // P2: undo user1
        // P3: stack empty → done
        var value = 0;
        sut.Execute(new RichUndoCommand("user1", () => value++, () => value--, allowUserAction: true));
        sut.Execute(new RichUndoCommand("internal1", () => value++, () => value--, allowUserAction: false));
        sut.Execute(new RichUndoCommand("internal2", () => value++, () => value--, allowUserAction: false));

        Assert.Equal(3, value);

        var result = sut.UndoToLastUserAction();

        Assert.True(result);
        Assert.Equal(0, value);
    }

    [Fact]
    public void UndoToLastUserAction_UndoesInternalsAndUserAction_StopsAtNextUser()
    {
        // Stack: [internal1, user2, user1] (top to bottom)
        // P1: undo internal1 (non-user)
        // P2: undo user2
        // P3: top is user1 (user) → stop
        var value = 0;
        sut.Execute(new RichUndoCommand("user1", () => value++, () => value--, allowUserAction: true));
        sut.Execute(new RichUndoCommand("user2", () => value++, () => value--, allowUserAction: true));
        sut.Execute(new RichUndoCommand("internal1", () => value++, () => value--, allowUserAction: false));

        Assert.Equal(3, value);

        var result = sut.UndoToLastUserAction();

        Assert.True(result);
        Assert.Equal(1, value);
    }

    [Fact]
    public void UndoToLastUserAction_AlsoUndoesInternalsBelowUserAction()
    {
        // Stack: [user2, internal1, internal2, user1] (top to bottom)
        // P1: top=user2 (user) → skip
        // P2: undo user2
        // P3: undo internal1, internal2 (non-user); top=user1 (user) → stop
        var value = 0;
        sut.Execute(new RichUndoCommand("user1", () => value++, () => value--, allowUserAction: true));
        sut.Execute(new RichUndoCommand("internal2", () => value++, () => value--, allowUserAction: false));
        sut.Execute(new RichUndoCommand("internal1", () => value++, () => value--, allowUserAction: false));
        sut.Execute(new RichUndoCommand("user2", () => value++, () => value--, allowUserAction: true));

        Assert.Equal(4, value);

        var result = sut.UndoToLastUserAction();

        Assert.True(result);
        Assert.Equal(1, value);
        Assert.Single(sut.UndoCommands);
    }

    [Fact]
    public void UndoToLastUserAction_PlainCommandTreatedAsUserAction()
    {
        // Stack: [internal, plain] (top to bottom)
        // P1: undo internal (non-user)
        // P2: undo plain (plain IUndoCommand = user)
        // P3: empty → done
        var value = 0;
        sut.Execute(new UndoCommand("plain", () => value++, () => value--));
        sut.Execute(new RichUndoCommand("internal", () => value++, () => value--, allowUserAction: false));

        var result = sut.UndoToLastUserAction();

        Assert.True(result);
        Assert.Equal(0, value);
    }

    [Fact]
    public void UndoToLastUserAction_ReturnsFalseWhenEmpty()
    {
        var result = sut.UndoToLastUserAction();

        Assert.False(result);
    }

    [Fact]
    public void UndoToLastUserAction_AllNonUser_UndoesAll()
    {
        var value = 0;
        sut.Execute(new RichUndoCommand("a", () => value++, () => value--, allowUserAction: false));
        sut.Execute(new RichUndoCommand("b", () => value++, () => value--, allowUserAction: false));

        var result = sut.UndoToLastUserAction();

        Assert.True(result);
        Assert.Equal(0, value);
        Assert.Empty(sut.UndoCommands);
    }

    [Fact]
    public void RedoToLastUserAction_RedoesUserActionOnly_LeavesTrailingInternals()
    {
        // Redo stack: [user1, internal1, internal2]
        // P1: skip (top is user)
        // P2: redo user1
        // Trailing internals left for next call
        var value = 0;
        sut.Execute(new RichUndoCommand("user1", () => value++, () => value--, allowUserAction: true));
        sut.Execute(new RichUndoCommand("internal1", () => value++, () => value--, allowUserAction: false));
        sut.Execute(new RichUndoCommand("internal2", () => value++, () => value--, allowUserAction: false));
        sut.UndoAll();
        Assert.Equal(0, value);

        var result = sut.RedoToLastUserAction();

        Assert.True(result);
        Assert.Equal(1, value);
        Assert.Equal(2, sut.RedoCommands.Count);
    }

    [Fact]
    public void RedoToLastUserAction_StopsAfterUserAction()
    {
        // Redo stack: [user1, internal1, user2]
        // P1: skip (top is user)
        // P2: redo user1
        var value = 0;
        sut.Execute(new RichUndoCommand("user1", () => value++, () => value--, allowUserAction: true));
        sut.Execute(new RichUndoCommand("internal1", () => value++, () => value--, allowUserAction: false));
        sut.Execute(new RichUndoCommand("user2", () => value++, () => value--, allowUserAction: true));
        sut.UndoAll();
        Assert.Equal(0, value);

        var result = sut.RedoToLastUserAction();

        Assert.True(result);
        Assert.Equal(1, value);
        Assert.Equal(2, sut.RedoCommands.Count);
    }

    [Fact]
    public void RedoToLastUserAction_LeadingInternals_RedoedWithNextUserAction()
    {
        // Redo stack: [internal1, user1, internal2]
        // P1: redo internal1 (non-user)
        // P2: redo user1 (user)
        // Trailing internal2 left for next call
        var value = 0;
        sut.Execute(new RichUndoCommand("internal1", () => value++, () => value--, allowUserAction: false));
        sut.Execute(new RichUndoCommand("user1", () => value++, () => value--, allowUserAction: true));
        sut.Execute(new RichUndoCommand("internal2", () => value++, () => value--, allowUserAction: false));
        sut.UndoAll();
        Assert.Equal(0, value);

        var result = sut.RedoToLastUserAction();

        Assert.True(result);
        Assert.Equal(2, value);
        Assert.Single(sut.RedoCommands);
    }

    [Fact]
    public void RedoToLastUserAction_ReturnsFalseWhenEmpty()
    {
        var result = sut.RedoToLastUserAction();

        Assert.False(result);
    }

    [Fact]
    public void RedoToLastUserAction_PlainCommandTreatedAsUserAction()
    {
        var value = 0;
        sut.Execute(new UndoCommand("plain", () => value++, () => value--));
        sut.UndoAll();

        var result = sut.RedoToLastUserAction();

        Assert.True(result);
        Assert.Equal(1, value);
    }

    [Fact]
    public void UndoRedo_UserAction_RoundTrip()
    {
        // Execute: user1, internal1, internal2
        // UndoToLastUserAction → undoes all three (user1 + trailing internals)
        // RedoToLastUserAction → redoes user1 only (internals left for next call)
        // RedoToLastUserAction → redoes internal1, internal2 (no user action left, undoes all remaining)
        var value = 0;
        sut.Execute(new RichUndoCommand("user1", () => value++, () => value--, allowUserAction: true));
        sut.Execute(new RichUndoCommand("internal1", () => value++, () => value--, allowUserAction: false));
        sut.Execute(new RichUndoCommand("internal2", () => value++, () => value--, allowUserAction: false));
        Assert.Equal(3, value);

        sut.UndoToLastUserAction();
        Assert.Equal(0, value);

        sut.RedoToLastUserAction();
        Assert.Equal(1, value);

        sut.RedoToLastUserAction();
        Assert.Equal(3, value);
    }

    [Fact]
    public void UndoRedo_UserAction_FullRoundTrip()
    {
        // Reproduce the user's exact scenario:
        // 1(u), 2(u), 3(u), autoA(i), autoB(i), 4(u), 5(u)
        var steps = new List<string>();
        void Set(string v) => steps.Add(v);

        sut.Execute(new RichUndoCommand("1", () => Set("do-1"), () => Set("undo-1"), allowUserAction: true));
        sut.Execute(new RichUndoCommand("2", () => Set("do-2"), () => Set("undo-2"), allowUserAction: true));
        sut.Execute(new RichUndoCommand("3", () => Set("do-3"), () => Set("undo-3"), allowUserAction: true));
        sut.Execute(new RichUndoCommand("autoA", () => Set("do-autoA"), () => Set("undo-autoA"), allowUserAction: false));
        sut.Execute(new RichUndoCommand("autoB", () => Set("do-autoB"), () => Set("undo-autoB"), allowUserAction: false));
        sut.Execute(new RichUndoCommand("4", () => Set("do-4"), () => Set("undo-4"), allowUserAction: true));
        sut.Execute(new RichUndoCommand("5", () => Set("do-5"), () => Set("undo-5"), allowUserAction: true));
        steps.Clear();

        // U2UA all the way up to Initial state
        // #1: undo 5 (P1:skip P2:undo-5 P3:top=4(u)→stop) → 6 remain
        sut.UndoToLastUserAction();
        Assert.Equal(6, sut.UndoCommands.Count);

        // #2: undo 4, autoB, autoA (P1:skip P2:undo-4 P3:undo-autoB,autoA;top=3(u)→stop) → 3 remain
        sut.UndoToLastUserAction();
        Assert.Equal(3, sut.UndoCommands.Count);

        // #3: undo 3 (P1:skip P2:undo-3 P3:top=2(u)→stop) → 2 remain
        sut.UndoToLastUserAction();
        Assert.Equal(2, sut.UndoCommands.Count);

        // #4: undo 2 (P1:skip P2:undo-2 P3:top=1(u)→stop) → 1 remain
        sut.UndoToLastUserAction();
        Assert.Single(sut.UndoCommands);

        // #5: undo 1 (P1:skip P2:undo-1 P3:empty→done) → Initial state
        sut.UndoToLastUserAction();
        Assert.Empty(sut.UndoCommands);
        Assert.Equal(7, sut.RedoCommands.Count);

        steps.Clear();

        // R2UA all the way down
        // Redo stack: [1(u), 2(u), 3(u), autoA(i), autoB(i), 4(u), 5(u)]

        // #1: redo 1 (P1:skip P2:redo-1)
        sut.RedoToLastUserAction();
        Assert.Single(sut.UndoCommands);
        Assert.Equal("1", sut.UndoCommands[0].Description);

        // #2: redo 2 (P1:skip P2:redo-2)
        sut.RedoToLastUserAction();
        Assert.Equal(2, sut.UndoCommands.Count);
        Assert.Equal("2", sut.UndoCommands[0].Description);

        // #3: redo 3 (P1:skip P2:redo-3) — trailing autoA,autoB left for next call
        sut.RedoToLastUserAction();
        Assert.Equal(3, sut.UndoCommands.Count);
        Assert.Equal("3", sut.UndoCommands[0].Description);

        // #4: redo autoA, autoB, 4 (P1:redo-autoA,autoB P2:redo-4)
        sut.RedoToLastUserAction();
        Assert.Equal(6, sut.UndoCommands.Count);
        Assert.Equal("4", sut.UndoCommands[0].Description);

        // #5: redo 5 (P1:skip P2:redo-5)
        sut.RedoToLastUserAction();
        Assert.Equal(7, sut.UndoCommands.Count);
        Assert.Equal("5", sut.UndoCommands[0].Description);
        Assert.Empty(sut.RedoCommands);
    }
}