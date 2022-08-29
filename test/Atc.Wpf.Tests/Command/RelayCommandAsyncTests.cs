// ReSharper disable RedundantAssignment
// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Tests.Command;

public class RelayCommandAsyncTests
{
    [Theory]
    [InlineData(0, true, 0)]
    [InlineData(1, true, 1)]
    [InlineData(2, true, 2)]
    [InlineData(0, false, 0)]
    [InlineData(1, false, 1)]
    [InlineData(2, false, 2)]
    [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "OK.")]
    public void RaiseCanExecuteChanged(int expected, bool canExecute, int registerOnChangeCount)
    {
        // Arrange
        var canExecuteChangedCalled = 0;
        var canExecuteChangedEventHandler = new EventHandler((_, _) => canExecuteChangedCalled++);

        var command = new RelayCommandAsync(MyTask, () => canExecute);

        for (var i = 0; i < registerOnChangeCount; i++)
        {
            command.CanExecuteChanged += canExecuteChangedEventHandler;
        }

        // Act
        command.RaiseCanExecuteChanged();

        // Ensure the invalidate is processed for 'CommandManager.InvalidateRequerySuggested'
        _ = Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));

        // Assert
        Assert.Equal(expected, canExecuteChangedCalled);
    }

    [Theory]
    [InlineData(true, true, null)]
    [InlineData(false, false, null)]
    [InlineData(true, true, 42)]
    [InlineData(false, false, 42)]
    public void CanExecute(bool expected, bool canExecute, object? parameter)
    {
        // Arrange
        var command = new RelayCommandAsync(MyTask, () => canExecute);

        // Act
        var actual = command.CanExecute(parameter);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Executed", true, null)]
    [InlineData("Not executed", false, null)]
    [InlineData("Executed", true, 42)]
    [InlineData("Not executed", false, 42)]
    public async Task Execute(string expected, bool canExecute, object? parameter)
    {
        // Arrange
        var actual = "Not executed";

        var command = new RelayCommandAsync(() => MyTask(ref actual, ref expected), () => canExecute);

        // Act
        await command.ExecuteAsync(parameter);

        // Assert
        Assert.Equal(expected, actual);
    }

    private static Task MyTask()
    {
        return Task.Delay(1);
    }

    [SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "OK.")]
    [SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
    [SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "OK.")]
    private static Task MyTask(ref string actual, ref string expected)
    {
        actual = expected;
        return Task.Delay(1);
    }
}