namespace Atc.Wpf.Tests.UndoRedo;

public sealed class UndoRedoGroupTests
{
    private readonly UndoRedoGroup sut = new();

    [Fact]
    public void AddStack_AddsToStacks()
    {
        var stack = new UndoRedoService();

        sut.AddStack(stack, "Doc1");

        Assert.Single(sut.Stacks);
        Assert.Same(stack, sut.Stacks[0]);
    }

    [Fact]
    public void RemoveStack_RemovesFromStacks()
    {
        var stack = new UndoRedoService();
        sut.AddStack(stack, "Doc1");

        sut.RemoveStack(stack);

        Assert.Empty(sut.Stacks);
    }

    [Fact]
    public void RemoveStack_ClearsActiveStack_WhenRemovedStackWasActive()
    {
        var stack = new UndoRedoService();
        sut.AddStack(stack, "Doc1");
        sut.ActiveStack = stack;

        sut.RemoveStack(stack);

        Assert.Null(sut.ActiveStack);
    }

    [Fact]
    public void RemoveStack_DoesNotClearActiveStack_WhenDifferentStackRemoved()
    {
        var stack1 = new UndoRedoService();
        var stack2 = new UndoRedoService();
        sut.AddStack(stack1, "Doc1");
        sut.AddStack(stack2, "Doc2");
        sut.ActiveStack = stack1;

        sut.RemoveStack(stack2);

        Assert.Same(stack1, sut.ActiveStack);
    }

    [Fact]
    public void ActiveStack_ThrowsArgumentException_WhenStackNotInGroup()
    {
        var stack = new UndoRedoService();

        Assert.Throws<ArgumentException>(() => sut.ActiveStack = stack);
    }

    [Fact]
    public void ActiveStack_AllowsNull()
    {
        sut.ActiveStack = null;

        Assert.Null(sut.ActiveStack);
    }

    [Fact]
    public void ActiveStackChanged_Fires_WhenActiveStackChanges()
    {
        var stack = new UndoRedoService();
        sut.AddStack(stack, "Doc1");
        var fired = false;
        sut.ActiveStackChanged += (_, _) => fired = true;

        sut.ActiveStack = stack;

        Assert.True(fired);
    }

    [Fact]
    public void ActiveStackChanged_DoesNotFire_WhenSetToSameStack()
    {
        var stack = new UndoRedoService();
        sut.AddStack(stack, "Doc1");
        sut.ActiveStack = stack;

        var fired = false;
        sut.ActiveStackChanged += (_, _) => fired = true;

        sut.ActiveStack = stack;

        Assert.False(fired);
    }

    [Fact]
    public void CanUndo_ReturnsFalse_WhenNoActiveStack()
    {
        Assert.False(sut.CanUndo);
    }

    [Fact]
    public void CanRedo_ReturnsFalse_WhenNoActiveStack()
    {
        Assert.False(sut.CanRedo);
    }

    [Fact]
    public void CanUndo_DelegatesToActiveStack()
    {
        var stack = new UndoRedoService();
        sut.AddStack(stack, "Doc1");
        sut.ActiveStack = stack;

        Assert.False(sut.CanUndo);

        stack.Execute(new UndoCommand("test", () => { }, () => { }));

        Assert.True(sut.CanUndo);
    }

    [Fact]
    public void CanRedo_DelegatesToActiveStack()
    {
        var stack = new UndoRedoService();
        sut.AddStack(stack, "Doc1");
        sut.ActiveStack = stack;
        stack.Execute(new UndoCommand("test", () => { }, () => { }));
        stack.Undo();

        Assert.True(sut.CanRedo);
    }

    [Fact]
    public void Undo_DelegatesToActiveStack()
    {
        var value = 0;
        var stack = new UndoRedoService();
        sut.AddStack(stack, "Doc1");
        sut.ActiveStack = stack;
        stack.Execute(new UndoCommand("inc", () => value++, () => value--));
        Assert.Equal(1, value);

        var result = sut.Undo();

        Assert.True(result);
        Assert.Equal(0, value);
    }

    [Fact]
    public void Redo_DelegatesToActiveStack()
    {
        var value = 0;
        var stack = new UndoRedoService();
        sut.AddStack(stack, "Doc1");
        sut.ActiveStack = stack;
        stack.Execute(new UndoCommand("inc", () => value++, () => value--));
        stack.Undo();

        var result = sut.Redo();

        Assert.True(result);
        Assert.Equal(1, value);
    }

    [Fact]
    public void Undo_ReturnsFalse_WhenNoActiveStack()
    {
        Assert.False(sut.Undo());
    }

    [Fact]
    public void Redo_ReturnsFalse_WhenNoActiveStack()
    {
        Assert.False(sut.Redo());
    }

    [Fact]
    public void PropertyChanged_Fires_WhenActiveStackStateChanges()
    {
        var stack = new UndoRedoService();
        sut.AddStack(stack, "Doc1");
        sut.ActiveStack = stack;

        var changedProperties = new List<string>();
        sut.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName!);

        stack.Execute(new UndoCommand("test", () => { }, () => { }));

        Assert.Contains(nameof(sut.CanUndo), changedProperties);
        Assert.Contains(nameof(sut.CanRedo), changedProperties);
    }

    [Fact]
    public void PropertyChanged_DoesNotFire_ForPreviousActiveStack()
    {
        var stack1 = new UndoRedoService();
        var stack2 = new UndoRedoService();
        sut.AddStack(stack1, "Doc1");
        sut.AddStack(stack2, "Doc2");
        sut.ActiveStack = stack1;

        // Switch to stack2
        sut.ActiveStack = stack2;

        var fired = false;
        sut.PropertyChanged += (_, _) => fired = true;

        // Mutate the old stack — should NOT raise property changed
        stack1.Execute(new UndoCommand("test", () => { }, () => { }));

        Assert.False(fired);
    }

    [Fact]
    public void BeginLinkedGroup_CollectsCommandsFromMultipleStacks()
    {
        var stack1 = new UndoRedoService();
        var stack2 = new UndoRedoService();
        sut.AddStack(stack1, "Doc1");
        sut.AddStack(stack2, "Doc2");

        var value1 = 0;
        var value2 = 0;

        using (sut.BeginLinkedGroup("Rename symbol"))
        {
            stack1.Execute(new UndoCommand("Rename in Doc1", () => value1++, () => value1--));
            stack2.Execute(new UndoCommand("Rename in Doc2", () => value2++, () => value2--));
        }

        Assert.Equal(1, value1);
        Assert.Equal(1, value2);
        Assert.True(stack1.CanUndo);
        Assert.True(stack2.CanUndo);
    }

    [Fact]
    public void LinkedGroup_UndoOnOneStack_UndoesPeerOnOtherStack()
    {
        var stack1 = new UndoRedoService();
        var stack2 = new UndoRedoService();
        sut.AddStack(stack1, "Doc1");
        sut.AddStack(stack2, "Doc2");

        var value1 = 0;
        var value2 = 0;

        using (sut.BeginLinkedGroup("Rename symbol"))
        {
            stack1.Execute(new UndoCommand("Rename in Doc1", () => value1++, () => value1--));
            stack2.Execute(new UndoCommand("Rename in Doc2", () => value2++, () => value2--));
        }

        // Undo on stack1 should also undo on stack2
        stack1.Undo();

        Assert.Equal(0, value1);
        Assert.Equal(0, value2);
        Assert.False(stack1.CanUndo);
        Assert.False(stack2.CanUndo);
    }

    [Fact]
    public void LinkedGroup_UndoOnSecondStack_UndoesPeerOnFirstStack()
    {
        var stack1 = new UndoRedoService();
        var stack2 = new UndoRedoService();
        sut.AddStack(stack1, "Doc1");
        sut.AddStack(stack2, "Doc2");

        var value1 = 0;
        var value2 = 0;

        using (sut.BeginLinkedGroup("Rename symbol"))
        {
            stack1.Execute(new UndoCommand("Rename in Doc1", () => value1++, () => value1--));
            stack2.Execute(new UndoCommand("Rename in Doc2", () => value2++, () => value2--));
        }

        // Undo on stack2 should also undo on stack1
        stack2.Undo();

        Assert.Equal(0, value1);
        Assert.Equal(0, value2);
    }

    [Fact]
    public void LinkedGroup_RedoOnOneStack_RedoesPeerOnOtherStack()
    {
        var stack1 = new UndoRedoService();
        var stack2 = new UndoRedoService();
        sut.AddStack(stack1, "Doc1");
        sut.AddStack(stack2, "Doc2");

        var value1 = 0;
        var value2 = 0;

        using (sut.BeginLinkedGroup("Rename symbol"))
        {
            stack1.Execute(new UndoCommand("Rename in Doc1", () => value1++, () => value1--));
            stack2.Execute(new UndoCommand("Rename in Doc2", () => value2++, () => value2--));
        }

        // Undo both via linked undo
        stack1.Undo();
        Assert.Equal(0, value1);
        Assert.Equal(0, value2);

        // Redo on stack1 should also redo on stack2
        stack1.Redo();

        Assert.Equal(1, value1);
        Assert.Equal(1, value2);
        Assert.True(stack1.CanUndo);
        Assert.True(stack2.CanUndo);
    }

    [Fact]
    public void LinkedGroup_NonLinkedCommandsAreUnaffected()
    {
        var stack1 = new UndoRedoService();
        var stack2 = new UndoRedoService();
        sut.AddStack(stack1, "Doc1");
        sut.AddStack(stack2, "Doc2");

        var value1 = 0;
        var value2 = 0;
        var unlinkedValue = 0;

        // Execute a non-linked command first
        stack1.Execute(new UndoCommand("Unlinked", () => unlinkedValue++, () => unlinkedValue--));

        using (sut.BeginLinkedGroup("Rename symbol"))
        {
            stack1.Execute(new UndoCommand("Rename in Doc1", () => value1++, () => value1--));
            stack2.Execute(new UndoCommand("Rename in Doc2", () => value2++, () => value2--));
        }

        // Undo the linked command
        stack1.Undo();

        // Linked commands should be undone
        Assert.Equal(0, value1);
        Assert.Equal(0, value2);

        // The unlinked command should still be on stack1's undo stack
        Assert.Equal(1, unlinkedValue);
        Assert.True(stack1.CanUndo);
    }

    [Fact]
    public void LinkedGroup_WithSingleStack_DoesNotLink()
    {
        var stack1 = new UndoRedoService();
        sut.AddStack(stack1, "Doc1");

        var value1 = 0;

        using (sut.BeginLinkedGroup("Single stack op"))
        {
            stack1.Execute(new UndoCommand("Op", () => value1++, () => value1--));
        }

        // Should work normally without linking
        stack1.Undo();
        Assert.Equal(0, value1);
    }

    [Fact]
    public void LinkedGroup_ThreeStacks_UndoAffectsAllPeers()
    {
        var stack1 = new UndoRedoService();
        var stack2 = new UndoRedoService();
        var stack3 = new UndoRedoService();
        sut.AddStack(stack1, "Doc1");
        sut.AddStack(stack2, "Doc2");
        sut.AddStack(stack3, "Doc3");

        var value1 = 0;
        var value2 = 0;
        var value3 = 0;

        using (sut.BeginLinkedGroup("Rename across 3 docs"))
        {
            stack1.Execute(new UndoCommand("R1", () => value1++, () => value1--));
            stack2.Execute(new UndoCommand("R2", () => value2++, () => value2--));
            stack3.Execute(new UndoCommand("R3", () => value3++, () => value3--));
        }

        // Undo on stack2 should undo all three
        stack2.Undo();

        Assert.Equal(0, value1);
        Assert.Equal(0, value2);
        Assert.Equal(0, value3);
    }

    [Fact]
    public void LinkedGroup_EmptyScope_DoesNotThrow()
    {
        var stack1 = new UndoRedoService();
        sut.AddStack(stack1, "Doc1");

        using (sut.BeginLinkedGroup("Empty"))
        {
            // No commands executed
        }

        Assert.False(stack1.CanUndo);
    }

    [Fact]
    [SuppressMessage("Reliability", "S1215:GC.Collect should not be called", Justification = "Required to test weak event reference behavior.")]
    public void WeakEvent_AllowsGroupToBeCollected_WhenNoStrongReferencesExist()
    {
        // Arrange: create a group, add a stack, and set it as active.
        // Then drop all strong references to the group.
        var stack = new UndoRedoService();
        var weakRef = CreateGroupAndGetWeakReference(stack);

        // Act: force garbage collection.
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Assert: the group should have been collected because the
        // WeakEventManager does not hold a strong reference to it.
        Assert.False(weakRef.TryGetTarget(out _));
    }

    /// <summary>
    /// Helper method that creates an <see cref="UndoRedoGroup"/>,
    /// subscribes it to the stack's events, and returns a weak reference.
    /// The method boundary ensures no strong reference survives on the stack.
    /// </summary>
    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private static WeakReference<UndoRedoGroup> CreateGroupAndGetWeakReference(
        UndoRedoService stack)
    {
        var group = new UndoRedoGroup();
        group.AddStack(stack, "Doc1");
        group.ActiveStack = stack;
        return new WeakReference<UndoRedoGroup>(group);
    }
}