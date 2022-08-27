// ReSharper disable RedundantAssignment
// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Tests.Command;

public class RelayCommandAsyncGenericTests
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

        var command = new RelayCommandAsync<string>(_ => MyTask(), _ => canExecute);

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
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void CanExecute(bool expected, bool canExecute)
    {
        // Arrange
        var command = new RelayCommandAsync<string>(_ => MyTask(), _ => canExecute);

        // Act
        var actual = command.CanExecute(null);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Executed", true, null)]
    //[InlineData("Not executed", false, null)]
    //[InlineData("Executed", true, 42)]
    //[InlineData("Not executed", false, 42)]
    public async Task Execute(string expected, bool canExecute, object? parameter)
    {
        // Arrange
        var actual = "Not executed";

        var command = new RelayCommandAsync<string>(_ => MyTask(expected, op => actual = op), _ => canExecute);

        // Act
        await command.ExecuteAsync(parameter?.ToString());

        // Assert
        Assert.Equal(expected, actual);
    }

    private static async Task<string> MyTask()
    {
        await Task.Delay(1);
        return "Hallo";
    }

    private delegate void OpDelegate(string op);

    [SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "OK.")]
    [SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
    [SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "OK.")]
    private static async Task<string> MyTask(string expected, OpDelegate callback)
    {
        await Task.Delay(1);
        callback(expected);
        return "Hallo";
    }
}