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

For more details on commands, see the [RelayCommand documentation](../Command/@Readme.md).

---

## Example: Using `ViewModelBase`

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

## Attributes for Property Source-Generation

### Quick Start Tips

```csharp
// Generates a property named "Name"
[ObservableProperty()] private string name;

// Generates a property named "MyName"
[ObservableProperty("MyName")] private string name;

// Generates a property named "MyName" and notifies FullName and Age
[ObservableProperty(nameof(MyName), nameof(FullName), nameof(Age))] private string name;

// Generates a property named "MyName" and notifies FullName and Age
[ObservableProperty(nameof(MyName), DependentProperties = [nameof(FullName), nameof(Age)])] private string name;

// Generates a property named "Name" and notifies FullName and Age
[ObservableProperty(DependentProperties = [nameof(FullName), nameof(Age)])]

// Notifies the property "Email"
[NotifyPropertyChangedFor(nameof(Email))]

// Notifies multiple properties
[NotifyPropertyChangedFor(nameof(FullName), nameof(Age))]
```

> Note: The `ObservableProperty` attribute automatically generates a property for a private field in a ViewModel class.
> The `NotifyPropertyChangedFor` attribute triggers change notifications for other properties when the annotated property changes. 
> Multiple properties can be notified, but an ObservableProperty must be defined for it to work.

## Attributes for RelayCommand Source-Generation

### Quick Start Tips

```csharp
// Generates a RelayCommand named "SaveCommand"
[RelayCommand()] public void Save();

// Generates a RelayCommand named "MySaveCommand"
[RelayCommand("MySave")] public void Save();

// Generates a RelayCommand that takes a string parameter
[RelayCommand()] public void Save(string text);

// Generates a RelayCommand with CanExecute function
[RelayCommand(CanExecute = nameof(CanSave))] public void Save();

// Generates an asynchronous RelayCommand
[RelayCommand()] public Task Save();

// Generates an asynchronous RelayCommand with async keyword
[RelayCommand()] public async Task Save();

// Generates an asynchronous RelayCommand named "MySaveCommand"
[RelayCommand("MySave")] public Task Save();

// Generates an asynchronous RelayCommand named "MySaveCommand" with async keyword
[RelayCommand("MySave")] public async Task Save();

// Generates an asynchronous RelayCommand that takes a string parameter
[RelayCommand()] public Task Save(string text);

// Generates an asynchronous RelayCommand with async keyword and string parameter
[RelayCommand()] public async Task Save(string text);

// Generates an asynchronous RelayCommand with CanExecute function
[RelayCommand(CanExecute = nameof(CanSave))] public Task Save();

// Generates an asynchronous RelayCommand with async keyword and CanExecute function
[RelayCommand(CanExecute = nameof(CanSave))] public async Task Save();

// Generates multi asynchronous RelayCommand with async keyword with multiple parameters
[RelayCommand("MyTestLeft", ParameterValues = [LeftTopRightBottomType.Left, 1])]
[RelayCommand("MyTestTop", ParameterValues = [LeftTopRightBottomType.Top, 1])]
[RelayCommand("MyTestRight", ParameterValues = [LeftTopRightBottomType.Right, 1])]
[RelayCommand("MyTestBottom", ParameterValues = [LeftTopRightBottomType.Bottom, 1])]
public Task TestDirection(LeftTopRightBottomType leftTopRightBottomType, int steps)

// Generates multi asynchronous RelayCommand with async keyword and CanExecute function with multiple parameters
[RelayCommand("MyTestLeft", CanExecute = nameof(CanTestDirection), ParameterValues = [LeftTopRightBottomType.Left, 1])]
[RelayCommand("MyTestTop", CanExecute = nameof(CanTestDirection), ParameterValues = [LeftTopRightBottomType.Top, 1])]
[RelayCommand("MyTestRight", CanExecute = nameof(CanTestDirection), ParameterValues = [LeftTopRightBottomType.Right, 1])]
[RelayCommand("MyTestBottom", CanExecute = nameof(CanTestDirection), ParameterValues = [LeftTopRightBottomType.Bottom, 1])]
public Task TestDirection(LeftTopRightBottomType leftTopRightBottomType, int steps)
```

> Note: The `RelayCommand` attribute is used to generate a `RelayCommand` property for a method in a ViewModel class.

---

## Example: TestViewModel

This is a simple example of a `TestViewModel` class with a single `ObservableProperty`.

### Humen made Code

```csharp
namespace TestNamespace;

public partial class TestViewModel : ViewModelBase
{
    [ObservableProperty]
    private string name;
}
```

### Generated Code

```csharp
#nullable enable

namespace TestNamespace;

public partial class TestViewModel
{
    public string Name
    {
        get => name;
        set
        {
            if (name == value)
            {
                return;
            }
            
            name = value;
            RaisePropertyChanged(nameof(Name));
        }
    }
}
```

---

## Example: PersonViewModel

This is a more complex example of a `PersonViewModel` class with multiple properties and commands.

### Humen made Code

```csharp
namespace TestNamespace;

public partial class PersonViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullName))]
    [Required]
    [MinLength(2)]
    private string firstName = "John";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullName), nameof(Age))]
    [NotifyPropertyChangedFor(nameof(Email))]
    [NotifyPropertyChangedFor(nameof(TheProperty))]
    private string? lastName = "Doe";

    [ObservableProperty]
    private int age = 27;

    [ObservableProperty]
    private string? email;

    [ObservableProperty("TheProperty", nameof(FullName), nameof(Age))]
    private string? myTestProperty;

    public string FullName => $"{FirstName} {LastName}";

    [RelayCommand]
    public void ShowData()
    {
        // TODO: Implement ShowData - it could be a dialog box
    }

    [RelayCommand(CanExecute = nameof(CanSaveHandler))]
    public void SaveHandler()
    {
        var dialogBox = new InfoDialogBox(
            Application.Current.MainWindow!,
            new DialogBoxSettings(DialogBoxType.Ok),
            "Hello to SaveHandler method");

        dialogBox.Show();
    }

    public bool CanSaveHandler()
    {
        // TODO: Implement validation
        return true;
    }
```

### Generated Code

```csharp
#nullable enable

namespace TestNamespace;

public partial class PersonViewModel
{
    public IRelayCommand ShowDataCommand => new RelayCommand(ShowData);

    public IRelayCommand SaveHandlerCommand => new RelayCommand(SaveHandler, CanSaveHandler);

    public string FirstName
    {
        get => firstName;
        set
        {
            if (firstName == value)
            {
                return;
            }

            firstName = value;
            RaisePropertyChanged(nameof(FirstName));
            RaisePropertyChanged(nameof(FullName));
        }
    }

    public string? LastName
    {
        get => lastName;
        set
        {
            if (lastName == value)
            {
                return;
            }

            lastName = value;
            RaisePropertyChanged(nameof(LastName));
            RaisePropertyChanged(nameof(FullName));
            RaisePropertyChanged(nameof(Age));
            RaisePropertyChanged(nameof(Email));
            RaisePropertyChanged(nameof(TheProperty));
        }
    }

    public int Age
    {
        get => age;
        set
        {
            if (age == value)
            {
                return;
            }

            age = value;
            RaisePropertyChanged(nameof(Age));
        }
    }

    public string? Email
    {
        get => email;
        set
        {
            if (email == value)
            {
                return;
            }

            email = value;
            RaisePropertyChanged(nameof(Email));
        }
    }

    public string? TheProperty
    {
        get => myTestProperty;
        set
        {
            if (myTestProperty == value)
            {
                return;
            }

            myTestProperty = value;
            RaisePropertyChanged(nameof(TheProperty));
            RaisePropertyChanged(nameof(FullName));
            RaisePropertyChanged(nameof(Age));
        }
    }
}
```