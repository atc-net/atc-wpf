# MVVM

The Windows Presentation Framework (WPF) takes full advantage of the Model-View-ViewModel (MVVM) pattern.

Therefore `Atc.Wpf` provide a good starting point for using MVVM.

| Tools set in the package | Description                                                                      |
|--------------------------| ---------------------------------------------------------------------------------|
| ViewModelBase            | A base class for a the ViewModels                                                |
| MainWindowViewModelBase  | A base class for a the MainWindow-ViewModel                                      |
| ViewModelDialogBase      | A base class for a the Dialog-ViewModel                                          |
| ObservableObject         | A base class for a observable class that implement a PropertyChangedEventHandler |
| RelayCommand             | Command with `CanExecute`                                                        |
| RelayCommand{T}          | Command with `CanExecute`                                                        |
| RelayCommandAsync        | Command with `CanExecute` as async                                               |
| RelayCommandAsync{T}     | Command with `CanExecute` as async                                               |

See more about [RelayCommand's](../Command/@Readme.md) and how to use them.


## Example for ViewModelBase usages

```csharp
public class MyViewModel : ViewModelBase
{
    private string myProperty;
    
    public string MyProperty
    {
        get => myProperty;
        set
        {
            if (name == value)
            {
                return;
            }
            
            myProperty = value;
            RaisePropertyChanged();
        }
    }
}
```

## Attributes

### Quick start
```csharp
// Generates a property with the name "Name"
[ObservableProperty()] private string name; 

// Generates a property with the name "MyName"
[ObservableProperty("myName"] private string name;

// Generates a property with the name "MyName" and also notifies the properties "FullName" and "Age"
[ObservableProperty("myName", nameof(FullName), nameof(Age))] private string name;

// Also notifies the property "Email"
[AlsoNotifyProperty(nameof(Email))]

// Also notifies the properties "FullName" and "Age"
[AlsoNotifyProperty(nameof(FullName), nameof(Age))]
```

## Example ViewModelBase with ObservablePropertyAttribute usages
```csharp
namespace TestNamespace;

public partial class TestViewModel : ViewModelBase
{
    [ObservableProperty]
    private string name;
}
```
### Generated code
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

## Example ViewModelBase with ObservablePropertyAttribute and AlsoNotifyPropertyAttribute usages
```csharp
namespace TestNamespace;

public partial class PersonViewModel : ViewModelBase
{
    [ObservableProperty()]
    [AlsoNotifyProperty(nameof(FullName))]
    private string firstName = string.Empty;
            
    [ObservableProperty()]
    [AlsoNotifyProperty(nameof(FullName), nameof(Age))]
    [AlsoNotifyProperty(nameof(Email))]
    [AlsoNotifyProperty(nameof(TheProperty))]
    private string? lastName;
            
    [ObservableProperty()]
    private int? age;
            
    [ObservableProperty()]
    private string? email;
            
    [ObservableProperty("TheProperty", nameof(FullName), nameof(Age))]
    private string? myTestProperty;
            
    public string FullName => $"{FirstName} {LastName}";
}
```
### Generated code
```csharp
#nullable enable
            
namespace TestNamespace;
            
public partial class PersonViewModel
{
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
        }
    }

    public int? Age
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

    public string? MyTestProperty
    {
        get => myTestProperty;
        set
        {
            if (myTestProperty == value)
            {
                return;
            }

            myTestProperty = value;
            RaisePropertyChanged(nameof(MyTestProperty));
        }
    }
}
```