using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Threading;
using Atc.Wpf.Command;
using Xunit;

namespace Atc.Wpf.Tests.Command
{
    public class RelayCommandGenericTests
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

            var command = new RelayCommand<string>(_ => { }, _ => canExecute);

            for (int i = 0; i < registerOnChangeCount; i++)
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
            var command = new RelayCommand<string>(_ => { }, _ => canExecute);

            // Act
            var actual = command.CanExecute(null);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Executed", true, null)]
        [InlineData("Not executed", false, null)]
        [InlineData("Executed", true, 42)]
        [InlineData("Not executed", false, 42)]
        public void Execute(string expected, bool canExecute, object? parameter)
        {
            // Arrange
            var actual = "Not executed";

            var command = new RelayCommand<string>(_ => { actual = expected; }, _ => canExecute);

            // Act
            command.Execute(parameter);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}