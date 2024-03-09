# RelayCommand's Component

The Atc `RelayCommand's` is a powerful tool designed for the Model-View-ViewModel (MVVM) pattern in .NET applications, 
especially within the context of WPF or similar XAML-based technologies. 

MVVM facilitates a clean separation of concerns between the application's user interface and its business logic. 
One of the key features of MVVM is the ability to bind commands, 
rather than events, to user actions like button clicks. 
This approach offers several advantages:

- **Decoupling of View and ViewModel:**
  Commands allow for a decoupling of the UI elements from their actions, making your code more modular and testable.
- **Enablement Logic:**
  Easily control the enablement of UI elements based on the command's ability to execute.
- **Asynchronous Support:**
  Execute long-running operations without blocking the UI, enhancing application responsiveness.

## Why Use RelayCommand's Over Events?

`RelayCommand's` provides a more robust, testable, and maintainable way of handling user interactions
compared to traditional event handlers.

By leveraging asynchronous operations,
`RelayCommand's` ensures that your application remains responsive, even during intensive tasks. 
This is especially crucial in modern applications where user experience is paramount.

The variants of `RelayCommand's`:

| Interface                | Concrete Implementation | Description |
|--------------------------|-------------------------|-------------|
| IRelayCommand            | RelayCommand            | A basic command implementation that executes synchronous actions. Useful for binding UI elements to actions that do not require asynchronous execution. |
| IRelayCommand < T >      | RelayCommand < T >      | A generic version of RelayCommand that allows for parameterized execution. This variant supports passing a parameter of type T to the command's execute and can-execute methods, facilitating more dynamic command operations. |
| IRelayCommandAsync       | RelayCommandAsync       | A asynchronous command implementation designed for executing tasks that return a Task. It enhances UI responsiveness by running operations in the background, without blocking the UI thread. |
| IRelayCommandAsync < T > | RelayCommandAsync < T > | A generic, asynchronous command variant that accepts a parameter of type T. This version is suitable for operations that require both asynchrony and parameterization, allowing the command to perform background operations with input data. |

## Implementation

The `RelayCommand's` extends the ICommand interface with asynchronous capabilities, enabling 
it to work seamlessly within an async/await programming model. It supports conditional execution 
through a can-execute function and integrates an error handling mechanism.

## Example Usage

Below is an example demonstrating how to implement and utilize `RelayCommand's` within a ViewModel,
showcasing the creation of commands and the binding of enablement logic:

```csharp
public class TestViewModel : ViewModelBase
{
    private bool isTestEnabled;

    public IRelayCommandAsync Test1Command => new RelayCommandAsync(Test1CommandHandler);

    public IRelayCommandAsync Test2Command => new RelayCommandAsync(Test2CommandHandler, () => IsTestEnabled);

    public IRelayCommandAsync<string> Test3Command => new RelayCommandAsync<string>(Test3CommandHandler);

    public IRelayCommandAsync<string> Test4Command => new RelayCommandAsync<string>(Test4CommandHandler, CanTest4CommandHandler);

    public bool IsTestEnabled
    {
        get => isTestEnabled;
        set
        {
            isTestEnabled = value;
            RaisePropertyChanged();

            // Ensure the command's CanExecute state is evaluated whenever this property changes.
            Test2Command.RaiseCanExecuteChanged();
        }
    }

    private async Task Test1CommandHandler()
    {
        // Simulate a long-running task
        await Task.Delay(1000);
        MessageBox.Show("Test1-command is executed", "Information", MessageBoxButton.OK);
    }

    private async Task Test2CommandHandler()
    {
        // Simulate a long-running task
        await Task.Delay(1000);
        MessageBox.Show("Test2-command is executed", "Information", MessageBoxButton.OK);
    }

    private async Task Test3CommandHandler(string parameter)
    {
        // Simulate a long-running task
        await Task.Delay(1000);
        MessageBox.Show($"Test3-command is executed with parameter: {parameter}", "Information", MessageBoxButton.OK);
    }

    private bool CanTest4CommandHandler(string obj)
    {
        return IsTestEnabled;
    }

    private async Task Test4CommandHandler(string obj)
    {
        // Simulate a long-running task
        await Task.Delay(1000, CancellationToken.None).ConfigureAwait(false);
        _ = MessageBox.Show("Test4-command is hit", $"CommandParameter: {obj}", MessageBoxButton.OK);
    }
}
```

> Note: All test command in the example can also be performed as without async/await by using RelayCommand's with the suffix Async.
