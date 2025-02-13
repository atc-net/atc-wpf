# MVVM

The Windows Presentation Foundation (WPF) fully supports the Model-View-ViewModel (MVVM) pattern.

The `Atc.Wpf` library provides a solid foundation for implementing MVVM effectively.

## Features

| Component                 | Description                                                                      |
|---------------------------|--------------------------------------------------------------------------------|
| `ViewModelBase`           | A base class for ViewModels.                                                   |
| `MainWindowViewModelBase` | A base class for the main window ViewModel.                                    |
| `ViewModelDialogBase`     | A base class for dialog ViewModels.                                            |
| `ObservableObject`        | A base class for observable objects implementing `INotifyPropertyChanged`.     |
| `RelayCommand`            | A command supporting `CanExecute`.                                             |
| `RelayCommand<T>`         | A command with a generic parameter and `CanExecute`.                           |
| `RelayCommandAsync`       | An asynchronous command supporting `CanExecute`.                               |
| `RelayCommandAsync<T>`    | An asynchronous command with a generic parameter and `CanExecute`.             |

For more details on commands, see the [RelayCommand documentation](../SourceGenerators/ViewModel.md).

---

### Getting started using `ViewModelBase`

```csharp
public class MyViewModel : ViewModelBase
{
    private string myProperty;

    public string MyProperty
    {
        get => myProperty;
        set
        {
            if (myProperty == value)
            {
                return;
            }

            myProperty = value;
            RaisePropertyChanged();
        }
    }
}
```

---
