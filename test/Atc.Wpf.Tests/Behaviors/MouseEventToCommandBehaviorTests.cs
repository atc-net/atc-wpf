namespace Atc.Wpf.Tests.Behaviors;

public sealed class MouseEventToCommandBehaviorTests
{
    [StaFact]
    public void LeftDoubleClickCommand_ExecutesOnlyWhenClickCountIsTwo()
    {
        // Arrange
        var element = new Border();
        var command = new CountingCommand();
        MouseEventToCommandBehavior.SetLeftDoubleClickCommand(element, command);

        // Act
        RaiseMouseDown(element, UIElement.PreviewMouseLeftButtonDownEvent, MouseButton.Left, clickCount: 1);
        var afterSingleClick = command.ExecuteCount;
        RaiseMouseDown(element, UIElement.PreviewMouseLeftButtonDownEvent, MouseButton.Left, clickCount: 2);

        // Assert
        Assert.Equal(0, afterSingleClick);
        Assert.Equal(1, command.ExecuteCount);
    }

    [StaFact]
    public void LeftDoubleClickCommand_TwoQuickSingleClicksDoNotExecute()
    {
        // Arrange
        var element = new Border();
        var command = new CountingCommand();
        MouseEventToCommandBehavior.SetLeftDoubleClickCommand(element, command);

        // Act
        RaiseMouseDown(element, UIElement.PreviewMouseLeftButtonDownEvent, MouseButton.Left, clickCount: 1);
        RaiseMouseDown(element, UIElement.PreviewMouseLeftButtonDownEvent, MouseButton.Left, clickCount: 1);

        // Assert
        Assert.Equal(0, command.ExecuteCount);
    }

    [StaFact]
    public void RightDoubleClickCommand_ExecutesOnlyWhenClickCountIsTwo()
    {
        // Arrange
        var element = new Border();
        var command = new CountingCommand();
        MouseEventToCommandBehavior.SetRightDoubleClickCommand(element, command);

        // Act
        RaiseMouseDown(element, UIElement.PreviewMouseRightButtonDownEvent, MouseButton.Right, clickCount: 1);
        var afterSingleClick = command.ExecuteCount;
        RaiseMouseDown(element, UIElement.PreviewMouseRightButtonDownEvent, MouseButton.Right, clickCount: 2);

        // Assert
        Assert.Equal(0, afterSingleClick);
        Assert.Equal(1, command.ExecuteCount);
    }

    [StaFact]
    public void RightDoubleClickCommand_LeftClickFollowedByRightClickDoesNotExecute()
    {
        // Arrange
        var element = new Border();
        var leftCommand = new CountingCommand();
        var rightCommand = new CountingCommand();
        MouseEventToCommandBehavior.SetLeftDoubleClickCommand(element, leftCommand);
        MouseEventToCommandBehavior.SetRightDoubleClickCommand(element, rightCommand);

        // Act
        RaiseMouseDown(element, UIElement.PreviewMouseLeftButtonDownEvent, MouseButton.Left, clickCount: 1);
        RaiseMouseDown(element, UIElement.PreviewMouseRightButtonDownEvent, MouseButton.Right, clickCount: 1);

        // Assert
        Assert.Equal(0, leftCommand.ExecuteCount);
        Assert.Equal(0, rightCommand.ExecuteCount);
    }

    [StaFact]
    public void LeftClickCommand_ExecutesOnEveryPress()
    {
        // Arrange
        var element = new Border();
        var command = new CountingCommand();
        MouseEventToCommandBehavior.SetLeftClickCommand(element, command);

        // Act
        RaiseMouseDown(element, UIElement.PreviewMouseLeftButtonDownEvent, MouseButton.Left, clickCount: 1);
        RaiseMouseDown(element, UIElement.PreviewMouseLeftButtonDownEvent, MouseButton.Left, clickCount: 2);

        // Assert
        Assert.Equal(2, command.ExecuteCount);
    }

    private static void RaiseMouseDown(
        UIElement element,
        RoutedEvent routedEvent,
        MouseButton button,
        int clickCount)
    {
        var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, timestamp: 0, button)
        {
            RoutedEvent = routedEvent,
        };

        typeof(MouseButtonEventArgs)
            .GetProperty(nameof(MouseButtonEventArgs.ClickCount))!
            .SetValue(args, clickCount);

        element.RaiseEvent(args);
    }

    private sealed class CountingCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add
            {
                // Not needed for the tests.
            }

            remove
            {
                // Not needed for the tests.
            }
        }

        public int ExecuteCount { get; private set; }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => ExecuteCount++;
    }
}