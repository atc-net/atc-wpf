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
    public void UndoCommands_ReturnsSameInstanceWhenUnchanged()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));

        var first = sut.UndoCommands;
        var second = sut.UndoCommands;

        Assert.Same(first, second);
    }

    [Fact]
    public void RedoCommands_ReturnsSameInstanceWhenUnchanged()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        sut.Undo();

        var first = sut.RedoCommands;
        var second = sut.RedoCommands;

        Assert.Same(first, second);
    }

    [Fact]
    public void UndoCommands_ReturnsNewInstanceAfterExecute()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        var first = sut.UndoCommands;

        sut.Execute(new UndoCommand("b", () => { }, () => { }));
        var second = sut.UndoCommands;

        Assert.NotSame(first, second);
    }

    [Fact]
    public void UndoCommands_ReturnsNewInstanceAfterUndo()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        sut.Execute(new UndoCommand("b", () => { }, () => { }));
        var first = sut.UndoCommands;

        sut.Undo();
        var second = sut.UndoCommands;

        Assert.NotSame(first, second);
    }

    [Fact]
    public void RedoCommands_ReturnsNewInstanceAfterUndo()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        var first = sut.RedoCommands;

        sut.Undo();
        var second = sut.RedoCommands;

        Assert.NotSame(first, second);
    }

    [Fact]
    public void UndoCommands_ReturnsNewInstanceAfterClear()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        var first = sut.UndoCommands;

        sut.Clear();
        var second = sut.UndoCommands;

        Assert.NotSame(first, second);
    }

    [Fact]
    public void RedoCommands_ReturnsNewInstanceAfterClear()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        sut.Undo();
        var first = sut.RedoCommands;

        sut.Clear();
        var second = sut.RedoCommands;

        Assert.NotSame(first, second);
    }

    [Fact]
    public void Execute_ObsoleteCommand_DoesNotPushToUndoStack()
    {
        var command = new ObsoleteAfterExecuteCommand("obsolete");

        sut.Execute(command);

        Assert.Empty(sut.UndoCommands);
        Assert.False(sut.CanUndo);
    }

    [Fact]
    public void Execute_NonObsoleteCommand_PushesToUndoStack()
    {
        var command = new UndoCommand("normal", () => { }, () => { });

        sut.Execute(command);

        Assert.Single(sut.UndoCommands);
        Assert.True(sut.CanUndo);
    }

    [Fact]
    public void Execute_ObsoleteCommand_DoesNotFireStateChanged()
    {
        var fired = false;
        sut.StateChanged += (_, _) => fired = true;

        sut.Execute(new ObsoleteAfterExecuteCommand("obsolete"));

        Assert.False(fired);
    }

    [Fact]
    public void Execute_ObsoleteCommand_DoesNotFireActionPerformed()
    {
        UndoRedoEventArgs? receivedArgs = null;
        sut.ActionPerformed += (_, e) => receivedArgs = e;

        sut.Execute(new ObsoleteAfterExecuteCommand("obsolete"));

        Assert.Null(receivedArgs);
    }

    [Fact]
    public void Add_ObsoleteCommand_DoesNotPushToUndoStack()
    {
        // Simulate a command that was executed externally and became obsolete.
        var command = new ObsoleteAfterExecuteCommand("obsolete");
        command.Execute();

        sut.Add(command);

        Assert.Empty(sut.UndoCommands);
    }

    [Fact]
    public void Execute_ObsoleteCommandInScope_DoesNotAddToGroup()
    {
        var value = 0;

        using (sut.BeginGroup("group"))
        {
            sut.Execute(new UndoCommand("normal", () => value++, () => value--));
            sut.Execute(new ObsoleteAfterExecuteCommand("obsolete"));
        }

        Assert.Equal(1, value);
        Assert.Single(sut.UndoCommands);

        sut.Undo();
        Assert.Equal(0, value);
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

    [Fact]
    public void RichUndoCommand_HasTimestampSetAfterConstruction()
    {
        var command = new RichUndoCommand("test", () => { }, () => { });

        Assert.NotEqual(default, command.Timestamp);
    }

    [Fact]
    public void RichUndoCommand_TimestampIsCloseToUtcNow()
    {
        var before = DateTimeOffset.UtcNow;
        var command = new RichUndoCommand("test", () => { }, () => { });
        var after = DateTimeOffset.UtcNow;

        Assert.InRange(command.Timestamp, before, after.AddSeconds(1));
    }

    [Fact]
    public void RichPropertyChangeCommand_HasTimestampSetAfterConstruction()
    {
        var value = 0;
        var command = new RichPropertyChangeCommand<int>("test", v => value = v, 0, 1);

        Assert.NotEqual(default, command.Timestamp);
    }

    [Fact]
    public void RichPropertyChangeCommand_TimestampIsCloseToUtcNow()
    {
        var value = 0;
        var before = DateTimeOffset.UtcNow;
        var command = new RichPropertyChangeCommand<int>("test", v => value = v, 0, 1);
        var after = DateTimeOffset.UtcNow;

        Assert.InRange(command.Timestamp, before, after.AddSeconds(1));
    }

    [Fact]
    public void Execute_MergeableSameMergeId_MergesIntoSingleEntry()
    {
        sut.Execute(new MergeableTestCommand(mergeId: 1, value: 10));
        sut.Execute(new MergeableTestCommand(mergeId: 1, value: 20));
        sut.Execute(new MergeableTestCommand(mergeId: 1, value: 30));

        Assert.Single(sut.UndoCommands);
    }

    [Fact]
    public void Execute_MergeableDifferentMergeId_DoesNotMerge()
    {
        sut.Execute(new MergeableTestCommand(mergeId: 1, value: 10));
        sut.Execute(new MergeableTestCommand(mergeId: 2, value: 20));

        Assert.Equal(2, sut.UndoCommands.Count);
    }

    [Fact]
    public void Execute_Mergeable_UpdatesExistingCommand()
    {
        sut.Execute(new MergeableTestCommand(mergeId: 1, value: 10));
        sut.Execute(new MergeableTestCommand(mergeId: 1, value: 42));

        var top = Assert.IsType<MergeableTestCommand>(sut.UndoCommands[0]);
        Assert.Equal(42, top.Value);
        Assert.Equal("Set to 42", top.Description);
    }

    [Fact]
    public void Execute_Mergeable_StillClearsRedoStack()
    {
        sut.Execute(new MergeableTestCommand(mergeId: 1, value: 10));
        sut.Undo();
        Assert.True(sut.CanRedo);

        sut.Execute(new MergeableTestCommand(mergeId: 1, value: 20));

        Assert.False(sut.CanRedo);
        Assert.Empty(sut.RedoCommands);
    }

    [Fact]
    public void Execute_NonMergeableFollowedByMergeable_DoesNotMerge()
    {
        sut.Execute(new UndoCommand("plain", () => { }, () => { }));
        sut.Execute(new MergeableTestCommand(mergeId: 1, value: 10));

        Assert.Equal(2, sut.UndoCommands.Count);
    }

    [Fact]
    public void Add_MergeableSameMergeId_MergesIntoSingleEntry()
    {
        sut.Add(new MergeableTestCommand(mergeId: 1, value: 10));
        sut.Add(new MergeableTestCommand(mergeId: 1, value: 20));
        sut.Add(new MergeableTestCommand(mergeId: 1, value: 30));

        Assert.Single(sut.UndoCommands);
        var top = Assert.IsType<MergeableTestCommand>(sut.UndoCommands[0]);
        Assert.Equal(30, top.Value);
    }

    [Fact]
    public void Undo_ApproverReturnsTrue_Succeeds()
    {
        var value = 0;
        sut.Execute(new UndoCommand("inc", () => value++, () => value--));
        sut.RegisterApprover(new TestApprover(approveUndo: true, approveRedo: true));

        var result = sut.Undo();

        Assert.True(result);
        Assert.Equal(0, value);
    }

    [Fact]
    public void Undo_ApproverReturnsFalse_BlocksUndo()
    {
        var value = 0;
        sut.Execute(new UndoCommand("inc", () => value++, () => value--));
        sut.RegisterApprover(new TestApprover(approveUndo: false, approveRedo: true));

        var result = sut.Undo();

        Assert.False(result);
        Assert.Equal(1, value);
        Assert.Single(sut.UndoCommands);
    }

    [Fact]
    public void Redo_ApproverReturnsFalse_BlocksRedo()
    {
        var value = 0;
        sut.Execute(new UndoCommand("inc", () => value++, () => value--));
        sut.Undo();
        sut.RegisterApprover(new TestApprover(approveUndo: true, approveRedo: false));

        var result = sut.Redo();

        Assert.False(result);
        Assert.Equal(0, value);
        Assert.Single(sut.RedoCommands);
    }

    [Fact]
    public void Undo_MultipleApproversOneVetoes_Blocked()
    {
        var value = 0;
        sut.Execute(new UndoCommand("inc", () => value++, () => value--));
        sut.RegisterApprover(new TestApprover(approveUndo: true, approveRedo: true));
        sut.RegisterApprover(new TestApprover(approveUndo: false, approveRedo: true));

        var result = sut.Undo();

        Assert.False(result);
        Assert.Equal(1, value);
    }

    [Fact]
    public void UnregisterApprover_RemovesApprover_UndoSucceeds()
    {
        var value = 0;
        sut.Execute(new UndoCommand("inc", () => value++, () => value--));
        var approver = new TestApprover(approveUndo: false, approveRedo: true);
        sut.RegisterApprover(approver);

        Assert.False(sut.Undo());

        sut.UnregisterApprover(approver);

        Assert.True(sut.Undo());
        Assert.Equal(0, value);
    }

    [Fact]
    public void UndoLevels_StopsAtVetoedCommand()
    {
        var value = 0;
        var cmd1 = new UndoCommand("a", () => value++, () => value--);
        var cmd2 = new UndoCommand("b", () => value++, () => value--);
        var cmd3 = new UndoCommand("c", () => value++, () => value--);
        sut.Execute(cmd1);
        sut.Execute(cmd2);
        sut.Execute(cmd3);
        Assert.Equal(3, value);

        // Veto cmd1 specifically (it will be the third undo attempt)
        sut.RegisterApprover(new SelectiveApprover(vetoCommand: cmd1));

        var result = sut.Undo(3);

        Assert.True(result);
        Assert.Equal(1, value);
        Assert.Single(sut.UndoCommands);
        Assert.Same(cmd1, sut.UndoCommands[0]);
    }

    private sealed class TestApprover(bool approveUndo, bool approveRedo) : IUndoOperationApprover
    {
        public bool ApproveUndo(IUndoCommand command) => approveUndo;

        public bool ApproveRedo(IUndoCommand command) => approveRedo;
    }

    private sealed class SelectiveApprover(IUndoCommand vetoCommand) : IUndoOperationApprover
    {
        public bool ApproveUndo(IUndoCommand command)
            => !ReferenceEquals(command, vetoCommand);

        public bool ApproveRedo(IUndoCommand command)
            => !ReferenceEquals(command, vetoCommand);
    }

    [Fact]
    public void CreateSnapshot_ReturnsNonNullWithCorrectName()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));

        var snapshot = sut.CreateSnapshot("checkpoint1");

        Assert.NotNull(snapshot);
        Assert.Equal("checkpoint1", snapshot.Name);
        Assert.NotEqual(default, snapshot.CreatedAt);
    }

    [Fact]
    public void Snapshots_ContainsCreatedSnapshot()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));

        var snapshot = sut.CreateSnapshot("snap");

        Assert.Single(sut.Snapshots);
        Assert.Same(snapshot, sut.Snapshots[0]);
    }

    [Fact]
    public void RestoreSnapshot_NavigatesBackToSnapshotPosition()
    {
        var value = 0;
        sut.Execute(new UndoCommand("a", () => value++, () => value--));
        var snapshot = sut.CreateSnapshot("after-a");

        sut.Execute(new UndoCommand("b", () => value++, () => value--));
        sut.Execute(new UndoCommand("c", () => value++, () => value--));
        Assert.Equal(3, value);

        sut.RestoreSnapshot(snapshot);

        Assert.Equal(1, value);
        Assert.Single(sut.UndoCommands);
    }

    [Fact]
    public void RestoreSnapshot_FromRedoPosition()
    {
        var value = 0;
        var cmdA = new UndoCommand("a", () => value++, () => value--);
        var cmdB = new UndoCommand("b", () => value++, () => value--);
        var cmdC = new UndoCommand("c", () => value++, () => value--);
        sut.Execute(cmdA);
        sut.Execute(cmdB);
        sut.Execute(cmdC);
        var snapshot = sut.CreateSnapshot("after-c");

        // Undo two commands so the snapshot position is in the redo stack
        sut.Undo();
        sut.Undo();
        Assert.Equal(1, value);

        sut.RestoreSnapshot(snapshot);

        Assert.Equal(3, value);
        Assert.Equal(3, sut.UndoCommands.Count);
    }

    [Fact]
    public void RestoreSnapshot_ToInitialState()
    {
        var snapshot = sut.CreateSnapshot("initial");

        var value = 0;
        sut.Execute(new UndoCommand("a", () => value++, () => value--));
        sut.Execute(new UndoCommand("b", () => value++, () => value--));
        Assert.Equal(2, value);

        sut.RestoreSnapshot(snapshot);

        Assert.Equal(0, value);
        Assert.Empty(sut.UndoCommands);
    }

    [Fact]
    public void RemoveSnapshot_RemovesFromList()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        var snapshot = sut.CreateSnapshot("snap");
        Assert.Single(sut.Snapshots);

        sut.RemoveSnapshot(snapshot);

        Assert.Empty(sut.Snapshots);
    }

    [Fact]
    public void Clear_ClearsSnapshots()
    {
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        sut.CreateSnapshot("snap");
        Assert.Single(sut.Snapshots);

        sut.Clear();

        Assert.Empty(sut.Snapshots);
    }

    [Fact]
    public void RestoreSnapshot_ThrowsWhenCommandTrimmed()
    {
        sut.MaxHistorySize = 2;
        sut.Execute(new UndoCommand("a", () => { }, () => { }));
        var snapshot = sut.CreateSnapshot("early");

        // Push enough commands to trim "a" from history
        sut.Execute(new UndoCommand("b", () => { }, () => { }));
        sut.Execute(new UndoCommand("c", () => { }, () => { }));
        sut.Execute(new UndoCommand("d", () => { }, () => { }));

        // The snapshot referencing "a" should have been removed during trimming
        Assert.Empty(sut.Snapshots);

        // Even if someone held onto the snapshot object, restoring should throw
        Assert.Throws<InvalidOperationException>(() => sut.RestoreSnapshot(snapshot));
    }

    [Fact]
    public void Undo_ByContext_UndoesToFirstMatchingCommand()
    {
        var drawCtx = new UndoContext("draw");
        var selectCtx = new UndoContext("select");
        var value = 0;

        sut.Execute(new RichUndoCommand("draw1", () => value++, () => value--, contexts: [drawCtx]));
        sut.Execute(new RichUndoCommand("select1", () => value++, () => value--, contexts: [selectCtx]));
        sut.Execute(new RichUndoCommand("draw2", () => value++, () => value--, contexts: [drawCtx]));
        Assert.Equal(3, value);

        // Undo(drawCtx) should find draw2 at top and undo it
        var result = sut.Undo(drawCtx);

        Assert.True(result);
        Assert.Equal(2, value);
        Assert.Equal(2, sut.UndoCommands.Count);
    }

    [Fact]
    public void Undo_ByContext_UndoesMultipleCommandsToReachMatch()
    {
        var drawCtx = new UndoContext("draw");
        var selectCtx = new UndoContext("select");
        var value = 0;

        sut.Execute(new RichUndoCommand("draw1", () => value++, () => value--, contexts: [drawCtx]));
        sut.Execute(new RichUndoCommand("select1", () => value++, () => value--, contexts: [selectCtx]));
        Assert.Equal(2, value);

        // Undo(drawCtx) should scan past select1 and find draw1, then UndoTo(draw1) undoes both
        var result = sut.Undo(drawCtx);

        Assert.True(result);
        Assert.Equal(0, value);
        Assert.Empty(sut.UndoCommands);
    }

    [Fact]
    public void Undo_ByContext_ReturnsFalseWhenNoMatchingCommands()
    {
        var drawCtx = new UndoContext("draw");
        var selectCtx = new UndoContext("select");
        var value = 0;

        sut.Execute(new RichUndoCommand("select1", () => value++, () => value--, contexts: [selectCtx]));

        var result = sut.Undo(drawCtx);

        Assert.False(result);
        Assert.Equal(1, value);
        Assert.Single(sut.UndoCommands);
    }

    [Fact]
    public void Undo_ByContext_ReturnsFalseWhenEmpty()
    {
        var ctx = new UndoContext("test");

        var result = sut.Undo(ctx);

        Assert.False(result);
    }

    [Fact]
    public void Redo_ByContext_RedoesToFirstMatchingCommand()
    {
        var drawCtx = new UndoContext("draw");
        var selectCtx = new UndoContext("select");
        var value = 0;

        sut.Execute(new RichUndoCommand("draw1", () => value++, () => value--, contexts: [drawCtx]));
        sut.Execute(new RichUndoCommand("select1", () => value++, () => value--, contexts: [selectCtx]));
        sut.Execute(new RichUndoCommand("draw2", () => value++, () => value--, contexts: [drawCtx]));
        sut.UndoAll();
        Assert.Equal(0, value);

        // Redo stack: [draw1, select1, draw2]
        // Redo(drawCtx) should find draw1 at top and redo it
        var result = sut.Redo(drawCtx);

        Assert.True(result);
        Assert.Equal(1, value);
        Assert.Single(sut.UndoCommands);
    }

    [Fact]
    public void Redo_ByContext_RedoesMultipleCommandsToReachMatch()
    {
        var drawCtx = new UndoContext("draw");
        var selectCtx = new UndoContext("select");
        var value = 0;

        sut.Execute(new RichUndoCommand("select1", () => value++, () => value--, contexts: [selectCtx]));
        sut.Execute(new RichUndoCommand("draw1", () => value++, () => value--, contexts: [drawCtx]));
        sut.UndoAll();
        Assert.Equal(0, value);

        // Redo stack: [select1, draw1]
        // Redo(drawCtx) should scan past select1 and find draw1, then RedoTo(draw1) redoes both
        var result = sut.Redo(drawCtx);

        Assert.True(result);
        Assert.Equal(2, value);
        Assert.Equal(2, sut.UndoCommands.Count);
    }

    [Fact]
    public void Redo_ByContext_ReturnsFalseWhenNoMatchingCommands()
    {
        var drawCtx = new UndoContext("draw");
        var selectCtx = new UndoContext("select");
        var value = 0;

        sut.Execute(new RichUndoCommand("select1", () => value++, () => value--, contexts: [selectCtx]));
        sut.UndoAll();

        var result = sut.Redo(drawCtx);

        Assert.False(result);
        Assert.Equal(0, value);
    }

    [Fact]
    public void GetUndoCommands_ByContext_ReturnsFilteredList()
    {
        var drawCtx = new UndoContext("draw");
        var selectCtx = new UndoContext("select");

        sut.Execute(new RichUndoCommand("draw1", () => { }, () => { }, contexts: [drawCtx]));
        sut.Execute(new RichUndoCommand("select1", () => { }, () => { }, contexts: [selectCtx]));
        sut.Execute(new RichUndoCommand("draw2", () => { }, () => { }, contexts: [drawCtx]));

        var drawCommands = sut.FindUndoCommands(drawCtx);

        Assert.Equal(2, drawCommands.Count);
        Assert.Equal("draw2", drawCommands[0].Description);
        Assert.Equal("draw1", drawCommands[1].Description);
    }

    [Fact]
    public void GetRedoCommands_ByContext_ReturnsFilteredList()
    {
        var drawCtx = new UndoContext("draw");
        var selectCtx = new UndoContext("select");

        sut.Execute(new RichUndoCommand("draw1", () => { }, () => { }, contexts: [drawCtx]));
        sut.Execute(new RichUndoCommand("select1", () => { }, () => { }, contexts: [selectCtx]));
        sut.Execute(new RichUndoCommand("draw2", () => { }, () => { }, contexts: [drawCtx]));
        sut.UndoAll();

        var drawCommands = sut.FindRedoCommands(drawCtx);

        Assert.Equal(2, drawCommands.Count);
        Assert.Equal("draw1", drawCommands[0].Description);
        Assert.Equal("draw2", drawCommands[1].Description);
    }

    [Fact]
    public void GetUndoCommands_ByContext_PlainCommandsNotIncluded()
    {
        var ctx = new UndoContext("draw");

        sut.Execute(new UndoCommand("plain", () => { }, () => { }));
        sut.Execute(new RichUndoCommand("draw1", () => { }, () => { }, contexts: [ctx]));

        var filtered = sut.FindUndoCommands(ctx);

        Assert.Single(filtered);
        Assert.Equal("draw1", filtered[0].Description);
    }

    [Fact]
    public void Undo_ByContext_PlainCommandsDoNotMatch()
    {
        var ctx = new UndoContext("draw");
        var value = 0;

        sut.Execute(new UndoCommand("plain", () => value++, () => value--));

        var result = sut.Undo(ctx);

        Assert.False(result);
        Assert.Equal(1, value);
    }

    [Fact]
    public void Undo_ByContext_CommandWithMultipleContextsMatches()
    {
        var drawCtx = new UndoContext("draw");
        var editCtx = new UndoContext("edit");
        var value = 0;

        sut.Execute(new RichUndoCommand("multi", () => value++, () => value--, contexts: [drawCtx, editCtx]));

        var resultDraw = sut.Undo(drawCtx);

        Assert.True(resultDraw);
        Assert.Equal(0, value);

        sut.Redo();
        Assert.Equal(1, value);

        var resultEdit = sut.Undo(editCtx);

        Assert.True(resultEdit);
        Assert.Equal(0, value);
    }

    [Fact]
    public void UndoContext_ToString_ReturnsName()
    {
        var ctx = new UndoContext("my-scope");

        Assert.Equal("my-scope", ctx.ToString());
    }

    [Fact]
    public void MaxHistoryMemory_DefaultIsZero_DoesNotTrimByMemory()
    {
        Assert.Equal(0, sut.MaxHistoryMemory);

        for (var i = 0; i < 5; i++)
        {
            sut.Execute(new MemoryAwareTestCommand($"cmd{i}", estimatedMemoryBytes: 1000));
        }

        Assert.Equal(5, sut.UndoCommands.Count);
        Assert.Equal(5000, sut.CurrentHistoryMemory);
    }

    [Fact]
    public void MaxHistoryMemory_TrimsOldestCommandsWhenExceeded()
    {
        sut.MaxHistoryMemory = 2500;
        sut.MaxHistorySize = 100;

        for (var i = 0; i < 5; i++)
        {
            sut.Execute(new MemoryAwareTestCommand($"cmd{i}", estimatedMemoryBytes: 1000));
        }

        // Budget is 2500, each command is 1000 bytes.
        // After pushing cmd4, total would be 5000 → trim oldest until ≤ 2500.
        // Should keep cmd4 (1000), cmd3 (1000) = 2000 ≤ 2500.
        // cmd2 would make it 3000 > 2500, so it gets trimmed too.
        // Wait — trimming removes from the bottom. Let's reason:
        // After all 5 pushes with count limit 100, count trim does nothing.
        // Memory trim: 5000 > 2500 → remove cmd0 (4000), still > 2500 → remove cmd1 (3000), still > 2500 → remove cmd2 (2000 ≤ 2500 → stop).
        Assert.Equal(2, sut.UndoCommands.Count);
        Assert.Equal("cmd4", sut.UndoCommands[0].Description);
        Assert.Equal("cmd3", sut.UndoCommands[1].Description);
        Assert.Equal(2000, sut.CurrentHistoryMemory);
    }

    [Fact]
    public void CurrentHistoryMemory_TracksCorrectlyAfterExecuteUndoClear()
    {
        sut.Execute(new MemoryAwareTestCommand("a", estimatedMemoryBytes: 500));
        Assert.Equal(500, sut.CurrentHistoryMemory);

        sut.Execute(new MemoryAwareTestCommand("b", estimatedMemoryBytes: 300));
        Assert.Equal(800, sut.CurrentHistoryMemory);

        sut.Undo();
        Assert.Equal(500, sut.CurrentHistoryMemory);

        sut.Clear();
        Assert.Equal(0, sut.CurrentHistoryMemory);
    }

    [Fact]
    public void CurrentHistoryMemory_NonMemoryAwareCommandsContributeZero()
    {
        sut.Execute(new UndoCommand("plain", () => { }, () => { }));

        Assert.Equal(0, sut.CurrentHistoryMemory);
    }

    [Fact]
    public void CurrentHistoryMemory_MixedMemoryAwareAndRegularCommands()
    {
        sut.MaxHistoryMemory = 1500;
        sut.MaxHistorySize = 100;

        sut.Execute(new UndoCommand("plain1", () => { }, () => { }));
        sut.Execute(new MemoryAwareTestCommand("mem1", estimatedMemoryBytes: 1000));
        sut.Execute(new UndoCommand("plain2", () => { }, () => { }));
        sut.Execute(new MemoryAwareTestCommand("mem2", estimatedMemoryBytes: 800));

        // Total memory = 1000 + 800 = 1800 > 1500.
        // Trim oldest: remove plain1 (0 bytes, still 1800 > 1500),
        // remove mem1 (1800 - 1000 = 800 ≤ 1500 → stop).
        Assert.Equal(2, sut.UndoCommands.Count);
        Assert.Equal("mem2", sut.UndoCommands[0].Description);
        Assert.Equal("plain2", sut.UndoCommands[1].Description);
        Assert.Equal(800, sut.CurrentHistoryMemory);
    }

    [Fact]
    public void CurrentHistoryMemory_IncreasesOnRedo()
    {
        sut.Execute(new MemoryAwareTestCommand("a", estimatedMemoryBytes: 500));
        sut.Undo();
        Assert.Equal(0, sut.CurrentHistoryMemory);

        sut.Redo();
        Assert.Equal(500, sut.CurrentHistoryMemory);
    }

    [Fact]
    public void AuditLogger_Execute_LogsEntryWithCorrectActionTypeAndDescription()
    {
        var logger = new UndoRedoAuditLogger();
        sut.AuditLogger = logger;

        sut.Execute(new UndoCommand("do something", () => { }, () => { }));

        Assert.Single(logger.Entries);
        Assert.Equal(UndoRedoActionType.Execute, logger.Entries[0].ActionType);
        Assert.Equal("do something", logger.Entries[0].Description);
        Assert.True(logger.Entries[0].Timestamp <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void AuditLogger_Undo_LogsEntry()
    {
        var logger = new UndoRedoAuditLogger();
        sut.AuditLogger = logger;
        sut.Execute(new UndoCommand("action", () => { }, () => { }));

        sut.Undo();

        Assert.Equal(2, logger.Entries.Count);
        Assert.Equal(UndoRedoActionType.Undo, logger.Entries[1].ActionType);
        Assert.Equal("action", logger.Entries[1].Description);
    }

    [Fact]
    public void AuditLogger_Redo_LogsEntry()
    {
        var logger = new UndoRedoAuditLogger();
        sut.AuditLogger = logger;
        sut.Execute(new UndoCommand("action", () => { }, () => { }));
        sut.Undo();

        sut.Redo();

        Assert.Equal(3, logger.Entries.Count);
        Assert.Equal(UndoRedoActionType.Redo, logger.Entries[2].ActionType);
        Assert.Equal("action", logger.Entries[2].Description);
    }

    [Fact]
    public void AuditLogger_Null_NoErrors()
    {
        sut.AuditLogger = null;

        sut.Execute(new UndoCommand("action", () => { }, () => { }));
        sut.Undo();
        sut.Redo();
        sut.Clear();

        Assert.Null(sut.AuditLogger);
    }

    [Fact]
    public void AuditLogger_MaxEntries_TrimsOldestEntries()
    {
        var logger = new UndoRedoAuditLogger(maxEntries: 3);
        sut.AuditLogger = logger;

        for (var i = 0; i < 5; i++)
        {
            sut.Execute(new UndoCommand($"cmd{i}", () => { }, () => { }));
        }

        Assert.Equal(3, logger.Entries.Count);
        Assert.Equal("cmd2", logger.Entries[0].Description);
        Assert.Equal("cmd3", logger.Entries[1].Description);
        Assert.Equal("cmd4", logger.Entries[2].Description);
    }

    [Fact]
    public void AuditLogger_Clear_LogsEntryAndClearClearsEntries()
    {
        var logger = new UndoRedoAuditLogger();
        sut.AuditLogger = logger;
        sut.Execute(new UndoCommand("action", () => { }, () => { }));

        sut.Clear();

        Assert.Equal(2, logger.Entries.Count);
        Assert.Equal(UndoRedoActionType.None, logger.Entries[1].ActionType);
        Assert.Equal("Clear", logger.Entries[1].Description);

        logger.Clear();

        Assert.Empty(logger.Entries);
    }

    [Fact]
    public void AuditLogger_Add_LogsExecuteEntry()
    {
        var logger = new UndoRedoAuditLogger();
        sut.AuditLogger = logger;

        sut.Add(new UndoCommand("added", () => { }, () => { }));

        Assert.Single(logger.Entries);
        Assert.Equal(UndoRedoActionType.Execute, logger.Entries[0].ActionType);
        Assert.Equal("added", logger.Entries[0].Description);
    }

    [Fact]
    public void SuspendRecording_ExecuteStillRunsCommand()
    {
        var value = 0;
        using (sut.SuspendRecording())
        {
            sut.Execute(new UndoCommand("inc", () => value++, () => value--));
        }

        Assert.Equal(1, value);
        Assert.Empty(sut.UndoCommands);
        Assert.False(sut.CanUndo);
    }

    [Fact]
    public void SuspendRecording_DoesNotFireEvents()
    {
        var actionFired = false;
        var stateFired = false;
        sut.ActionPerformed += (_, _) => actionFired = true;
        sut.StateChanged += (_, _) => stateFired = true;

        using (sut.SuspendRecording())
        {
            sut.Execute(new UndoCommand("test", () => { }, () => { }));
        }

        Assert.False(actionFired);
        Assert.False(stateFired);
    }

    [Fact]
    public void SuspendRecording_AfterDispose_RecordingResumes()
    {
        using (sut.SuspendRecording())
        {
            sut.Execute(new UndoCommand("suspended", () => { }, () => { }));
        }

        sut.Execute(new UndoCommand("recorded", () => { }, () => { }));

        Assert.Single(sut.UndoCommands);
        Assert.Equal("recorded", sut.UndoCommands[0].Description);
    }

    [Fact]
    public void SuspendRecording_NestedSuspend_StillSuspendedAfterSingleDispose()
    {
        var outer = sut.SuspendRecording();
        var inner = sut.SuspendRecording();

        inner.Dispose();

        Assert.True(sut.IsRecordingSuspended);

        sut.Execute(new UndoCommand("still-suspended", () => { }, () => { }));
        Assert.Empty(sut.UndoCommands);

        outer.Dispose();

        Assert.False(sut.IsRecordingSuspended);

        sut.Execute(new UndoCommand("recorded", () => { }, () => { }));
        Assert.Single(sut.UndoCommands);
    }

    [Fact]
    public void SuspendRecording_AddIsIgnoredDuringSuspension()
    {
        using (sut.SuspendRecording())
        {
            sut.Add(new UndoCommand("ignored", () => { }, () => { }));
        }

        Assert.Empty(sut.UndoCommands);
    }

    [Fact]
    public void IsRecordingSuspended_ReturnsFalseByDefault()
    {
        Assert.False(sut.IsRecordingSuspended);
    }

    [Fact]
    public void IsRecordingSuspended_ReturnsTrueWhenSuspended()
    {
        using (sut.SuspendRecording())
        {
            Assert.True(sut.IsRecordingSuspended);
        }
    }

    [Fact]
    public void IsRecordingSuspended_ReturnsFalseAfterDispose()
    {
        var scope = sut.SuspendRecording();
        Assert.True(sut.IsRecordingSuspended);

        scope.Dispose();

        Assert.False(sut.IsRecordingSuspended);
    }

    [Fact]
    public void SuspendRecording_DoubleDispose_DoesNotUnderflow()
    {
        var scope = sut.SuspendRecording();
        scope.Dispose();
        scope.Dispose();

        Assert.False(sut.IsRecordingSuspended);

        sut.Execute(new UndoCommand("test", () => { }, () => { }));
        Assert.Single(sut.UndoCommands);
    }

    [Fact]
    public void SuspendRecording_DoesNotClearRedoStack()
    {
        var value = 0;
        sut.Execute(new UndoCommand("a", () => value++, () => value--));
        sut.Undo();
        Assert.True(sut.CanRedo);

        using (sut.SuspendRecording())
        {
            sut.Execute(new UndoCommand("suspended", () => { }, () => { }));
        }

        Assert.True(sut.CanRedo);
    }

    [Fact]
    public void AllowNonLinearHistory_DefaultIsFalse()
    {
        Assert.False(sut.AllowNonLinearHistory);
    }

    [Fact]
    public void Execute_AfterUndo_WhenNonLinearDisabled_ClearsRedo()
    {
        var value = 0;
        sut.Execute(new UndoCommand("a", () => value++, () => value--));
        sut.Undo();
        Assert.True(sut.CanRedo);

        sut.Execute(new UndoCommand("b", () => value = 10, () => value = 0));

        Assert.False(sut.CanRedo);
        Assert.Empty(sut.RedoBranches);
    }

    [Fact]
    public void Execute_AfterUndo_WhenNonLinearEnabled_SavesRedoAsBranch()
    {
        sut.AllowNonLinearHistory = true;
        var value = 0;
        var cmdA = new UndoCommand("a", () => value++, () => value--);
        sut.Execute(cmdA);
        sut.Undo();
        Assert.True(sut.CanRedo);

        sut.Execute(new UndoCommand("b", () => value = 10, () => value = 0));

        Assert.False(sut.CanRedo);
        Assert.Single(sut.RedoBranches);
        Assert.Single(sut.RedoBranches[0]);
        Assert.Same(cmdA, sut.RedoBranches[0][0]);
    }

    [Fact]
    public void RedoBranches_ContainsMultipleSavedBranches()
    {
        sut.AllowNonLinearHistory = true;
        var value = 0;
        var cmdA = new UndoCommand("a", () => value++, () => value--);
        var cmdB = new UndoCommand("b", () => value++, () => value--);

        sut.Execute(cmdA);
        sut.Undo();
        sut.Execute(new UndoCommand("x", () => { }, () => { }));
        Assert.Single(sut.RedoBranches);

        sut.Execute(cmdB);
        sut.Undo();
        sut.Undo();
        sut.Execute(new UndoCommand("y", () => { }, () => { }));

        Assert.Equal(2, sut.RedoBranches.Count);
    }

    [Fact]
    public void SwitchRedoBranch_RestoresBranchAsCurrentRedo()
    {
        sut.AllowNonLinearHistory = true;
        var value = 0;
        var cmdA = new UndoCommand("a", () => value++, () => value--);

        sut.Execute(cmdA);
        sut.Undo();
        Assert.True(sut.CanRedo);

        sut.Execute(new UndoCommand("b", () => value = 10, () => value = 0));

        Assert.Single(sut.RedoBranches);
        Assert.False(sut.CanRedo);

        sut.SwitchRedoBranch(0);

        Assert.True(sut.CanRedo);
        Assert.Single(sut.RedoCommands);
        Assert.Same(cmdA, sut.RedoCommands[0]);
    }

    [Fact]
    public void SwitchRedoBranch_SavesCurrentRedoAsNewBranch()
    {
        sut.AllowNonLinearHistory = true;
        var value = 0;
        var cmdA = new UndoCommand("a", () => value++, () => value--);
        var cmdB = new UndoCommand("b", () => value++, () => value--);

        sut.Execute(cmdA);
        sut.Undo();
        sut.Execute(new UndoCommand("x", () => { }, () => { }));

        sut.Execute(cmdB);
        sut.Undo();

        Assert.Single(sut.RedoBranches);
        Assert.True(sut.CanRedo);

        sut.SwitchRedoBranch(0);

        Assert.Single(sut.RedoBranches);
        Assert.Same(cmdB, sut.RedoBranches[0][0]);
        Assert.Same(cmdA, sut.RedoCommands[0]);
    }

    [Fact]
    public void SwitchRedoBranch_WhenNonLinearDisabled_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => sut.SwitchRedoBranch(0));
    }

    [Fact]
    public void SwitchRedoBranch_InvalidIndex_Throws()
    {
        sut.AllowNonLinearHistory = true;

        Assert.Throws<ArgumentOutOfRangeException>(() => sut.SwitchRedoBranch(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => sut.SwitchRedoBranch(-1));
    }

    [Fact]
    public void Clear_ClearsBranches()
    {
        sut.AllowNonLinearHistory = true;
        var value = 0;
        sut.Execute(new UndoCommand("a", () => value++, () => value--));
        sut.Undo();
        sut.Execute(new UndoCommand("b", () => { }, () => { }));
        Assert.Single(sut.RedoBranches);

        sut.Clear();

        Assert.Empty(sut.RedoBranches);
    }

    [Fact]
    public void Add_AfterUndo_WhenNonLinearEnabled_SavesRedoAsBranch()
    {
        sut.AllowNonLinearHistory = true;
        var value = 0;
        var cmdA = new UndoCommand("a", () => value++, () => value--);
        sut.Execute(cmdA);
        sut.Undo();
        Assert.True(sut.CanRedo);

        sut.Add(new UndoCommand("b", () => { }, () => { }));

        Assert.False(sut.CanRedo);
        Assert.Single(sut.RedoBranches);
        Assert.Same(cmdA, sut.RedoBranches[0][0]);
    }

    [Fact]
    public void SaveHistory_LoadHistory_RoundTripsUndoCommands()
    {
        var cmd1 = new SerializableTestCommand(1);
        var cmd2 = new SerializableTestCommand(2);
        sut.Execute(cmd1);
        sut.Execute(cmd2);

        using var stream = new MemoryStream();
        sut.SaveHistory(stream);
        stream.Position = 0;

        var target = new UndoRedoService();
        target.LoadHistory(stream, new TestDeserializer());

        Assert.Equal(2, target.UndoCommands.Count);
        var loaded1 = Assert.IsType<SerializableTestCommand>(target.UndoCommands[0]);
        var loaded2 = Assert.IsType<SerializableTestCommand>(target.UndoCommands[1]);
        Assert.Equal(2, loaded1.Value);
        Assert.Equal(1, loaded2.Value);
    }

    [Fact]
    public void SaveHistory_LoadHistory_PreservesRedoCommands()
    {
        var cmd1 = new SerializableTestCommand(10);
        var cmd2 = new SerializableTestCommand(20);
        var cmd3 = new SerializableTestCommand(30);
        sut.Execute(cmd1);
        sut.Execute(cmd2);
        sut.Execute(cmd3);

        // Undo two so they move to redo stack
        sut.Undo();
        sut.Undo();

        using var stream = new MemoryStream();
        sut.SaveHistory(stream);
        stream.Position = 0;

        var target = new UndoRedoService();
        target.LoadHistory(stream, new TestDeserializer());

        Assert.Single(target.UndoCommands);
        Assert.Equal(2, target.RedoCommands.Count);

        var undoCmd = Assert.IsType<SerializableTestCommand>(target.UndoCommands[0]);
        Assert.Equal(10, undoCmd.Value);

        var redoFirst = Assert.IsType<SerializableTestCommand>(target.RedoCommands[0]);
        var redoSecond = Assert.IsType<SerializableTestCommand>(target.RedoCommands[1]);
        Assert.Equal(20, redoFirst.Value);
        Assert.Equal(30, redoSecond.Value);
    }

    [Fact]
    public void SaveHistory_SkipsNonSerializableCommands()
    {
        sut.Execute(new UndoCommand("plain", () => { }, () => { }));
        sut.Execute(new SerializableTestCommand(42));
        sut.Execute(new UndoCommand("plain2", () => { }, () => { }));

        using var stream = new MemoryStream();
        sut.SaveHistory(stream);
        stream.Position = 0;

        var target = new UndoRedoService();
        target.LoadHistory(stream, new TestDeserializer());

        Assert.Single(target.UndoCommands);
        var loaded = Assert.IsType<SerializableTestCommand>(target.UndoCommands[0]);
        Assert.Equal(42, loaded.Value);
    }

    [Fact]
    public void LoadHistory_ClearsExistingState()
    {
        sut.Execute(new UndoCommand("existing", () => { }, () => { }));
        sut.Execute(new UndoCommand("existing2", () => { }, () => { }));

        // Save an empty history from a fresh service
        using var stream = new MemoryStream();
        new UndoRedoService().SaveHistory(stream);
        stream.Position = 0;

        sut.LoadHistory(stream, new TestDeserializer());

        Assert.Empty(sut.UndoCommands);
        Assert.Empty(sut.RedoCommands);
        Assert.False(sut.CanUndo);
        Assert.False(sut.CanRedo);
    }

    private sealed class SerializableTestCommand : ISerializableUndoCommand
    {
        public SerializableTestCommand(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public string TypeId => "test-int";

        public string Description => $"Set {Value}";

        public void Execute()
        {
            // No-op for testing.
        }

        public void UnExecute()
        {
            // No-op for testing.
        }

        public byte[] Serialize()
            => BitConverter.GetBytes(Value);
    }

    private sealed class TestDeserializer : IUndoCommandDeserializer
    {
        public IUndoCommand? Deserialize(
            string typeId,
            byte[] data)
        {
            if (typeId == "test-int")
            {
                var value = BitConverter.ToInt32(data, 0);
                return new SerializableTestCommand(value);
            }

            return null;
        }
    }
}