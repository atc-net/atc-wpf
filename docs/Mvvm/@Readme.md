# 🧱 MVVM in WPF

Windows Presentation Foundation (WPF) fully supports the **Model-View-ViewModel (MVVM)** pattern, which promotes a clear separation of concerns between the UI and business logic.

The **Atc.Wpf** library includes the [Atc.XamlToolkit](https://github.com/atc-net/atc-xaml-toolkit) MVVM infrastructure as a dependency, providing a robust foundation for implementing MVVM effectively, reducing boilerplate code and simplifying development.

## ⚙️ Features

The MVVM infrastructure is provided by the **Atc.XamlToolkit** and **Atc.XamlToolkit.Wpf** packages, which are included as dependencies of `Atc.Wpf`:

| 🧩 Component             | 📦 Source Namespace       | 📋 Description                                                                 |
|---------------------------|---------------------------|--------------------------------------------------------------------------------|
| `ViewModelBase`           | Atc.XamlToolkit.Mvvm      | A base class for ViewModels.                                                   |
| `MainWindowViewModelBase` | Atc.XamlToolkit.Mvvm      | A base class for the main window ViewModel.                                    |
| `ViewModelDialogBase`     | Atc.XamlToolkit.Mvvm      | A base class for dialog ViewModels.                                            |
| `ObservableObject`        | Atc.XamlToolkit.Mvvm      | A base class for observable objects implementing `INotifyPropertyChanged`.     |
| `RelayCommand`            | Atc.XamlToolkit.Command   | A command supporting `CanExecute`.                                             |
| `RelayCommand<T>`         | Atc.XamlToolkit.Command   | A command with a generic parameter and `CanExecute`.                           |
| `RelayCommandAsync`       | Atc.XamlToolkit.Command   | An asynchronous command supporting `CanExecute`.                               |
| `RelayCommandAsync<T>`    | Atc.XamlToolkit.Command   | An asynchronous command with a generic parameter and `CanExecute`.             |

📖 For detailed information about source generators and commands, refer to the [ViewModel Source Generation documentation](../SourceGenerators/ViewModel.md).

🔁 Migrating an existing project from hand-rolled `INotifyPropertyChanged` / `RelayCommand` boilerplate? See the [Migration guide](Migration.md).

📦 For more information about the MVVM infrastructure, see the [Atc.XamlToolkit repository](https://github.com/atc-net/atc-xaml-toolkit).

---

### 🚀 Getting started using `ViewModelBase`

Below is a simple example demonstrating how to create a ViewModel using `ViewModelBase`:

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
