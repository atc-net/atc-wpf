namespace Atc.Wpf.UndoRedo.Tests;

public sealed class UndoRedoHistoryViewModelTests
{
    [Fact]
    public void HistoryItems_IsEmpty_WhenServiceNotSet()
    {
        // Arrange
        var vm = new UndoRedoHistoryViewModel();

        // Act & Assert
        vm.HistoryItems.Should().BeEmpty();
    }

    [Fact]
    public void HistoryItems_ContainsInitialStateRow_WhenServiceSetWithNoCommands()
    {
        // Arrange
        var vm = new UndoRedoHistoryViewModel
        {
            UndoRedoService = new UndoRedoService(),
        };

        // Act & Assert
        vm.HistoryItems.Should().HaveCount(1);
        vm.HistoryItems[0].Command.Should().BeNull();
        vm.HistoryItems[0].IsHighlighted.Should().BeTrue();
    }

    [Fact]
    public void HistoryItems_IncludesExecutedCommands()
    {
        // Arrange
        var service = new UndoRedoService();
        var vm = new UndoRedoHistoryViewModel { UndoRedoService = service };

        // Act
        service.Execute(new UndoCommand("A", () => { }, () => { }));
        service.Execute(new UndoCommand("B", () => { }, () => { }));

        // Assert: 1 initial + 2 undo rows
        vm.HistoryItems.Should().HaveCount(3);
        vm.HistoryItems[1].Description.Should().Be("A");
        vm.HistoryItems[2].Description.Should().Be("B");
        vm.HistoryItems[2].IsHighlighted.Should().BeTrue();
    }

    [Fact]
    public void HistoryItems_MarksRedoRows_AfterUndo()
    {
        // Arrange
        var service = new UndoRedoService();
        var vm = new UndoRedoHistoryViewModel { UndoRedoService = service };
        service.Execute(new UndoCommand("A", () => { }, () => { }));
        service.Execute(new UndoCommand("B", () => { }, () => { }));

        // Act
        service.Undo();

        // Assert: A is current (highlighted), B is redo
        vm.HistoryItems.Should().HaveCount(3);
        vm.HistoryItems[1].Description.Should().Be("A");
        vm.HistoryItems[1].IsHighlighted.Should().BeTrue();
        vm.HistoryItems[2].Description.Should().Be("B");
        vm.HistoryItems[2].IsRedo.Should().BeTrue();
    }

    [Fact]
    public void UndoRedoService_Setter_UnsubscribesFromPreviousService()
    {
        // Arrange
        var first = new UndoRedoService();
        var second = new UndoRedoService();
        var vm = new UndoRedoHistoryViewModel { UndoRedoService = first };

        // Act: switch service and execute on the old one
        vm.UndoRedoService = second;
        first.Execute(new UndoCommand("Old", () => { }, () => { }));

        // Assert: HistoryItems reflects only the second (empty) service
        vm.HistoryItems.Should().HaveCount(1);
        vm.HistoryItems[0].Command.Should().BeNull();
    }

    [Fact]
    public void UndoCommand_CanExecute_ReflectsServiceCanUndo()
    {
        // Arrange
        var service = new UndoRedoService();
        var vm = new UndoRedoHistoryViewModel { UndoRedoService = service };

        // Assert: empty service
        vm.UndoCommand.CanExecute(null).Should().BeFalse();

        // Act
        service.Execute(new UndoCommand("A", () => { }, () => { }));

        // Assert: one command → can undo
        vm.UndoCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void MarkSavedCommand_CanExecute_OnlyWhenDirty()
    {
        // Arrange
        var service = new UndoRedoService();
        var vm = new UndoRedoHistoryViewModel { UndoRedoService = service };

        // Assert: clean state
        vm.MarkSavedCommand.CanExecute(null).Should().BeFalse();

        // Act
        service.Execute(new UndoCommand("A", () => { }, () => { }));

        // Assert: dirty after command
        vm.MarkSavedCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void NavigateTo_InitialStateRow_UndoesAll()
    {
        // Arrange
        var state = 0;
        var service = new UndoRedoService();
        var vm = new UndoRedoHistoryViewModel { UndoRedoService = service };
        service.Execute(new UndoCommand("+1", () => state++, () => state--));
        service.Execute(new UndoCommand("+1", () => state++, () => state--));

        // Act
        vm.NavigateToCommand.Execute(vm.HistoryItems[0]);

        // Assert
        state.Should().Be(0);
        service.CanUndo.Should().BeFalse();
        service.CanRedo.Should().BeTrue();
    }

    [Fact]
    public void NavigateTo_RedoRow_RedoesToThatCommand()
    {
        // Arrange
        var state = 0;
        var service = new UndoRedoService();
        var vm = new UndoRedoHistoryViewModel { UndoRedoService = service };
        service.Execute(new UndoCommand("+1", () => state++, () => state--));
        service.Execute(new UndoCommand("+1", () => state++, () => state--));
        service.UndoAll();

        // Act: navigate to last redo row
        var lastRedo = vm.HistoryItems.Last(x => x.IsRedo);
        vm.NavigateToCommand.Execute(lastRedo);

        // Assert: all commands redone
        state.Should().Be(2);
        service.CanRedo.Should().BeFalse();
    }
}