# ⚙️ ViewModel with Source Generation

The **Atc.Wpf Source Generators** simplify ViewModel development by reducing boilerplate code for properties and commands. With attributes like `ObservableProperty` and `RelayCommand`, you can focus on business logic while automatically handling property change notifications and command implementations.

---

## 🚀 Setting Up Your First ViewModel

### ✨ Creating a Simple ViewModel

Let's start by defining a ViewModel using source generators.

```csharp
public partial class TestViewModel : ViewModelBase
{
    [ObservableProperty]
    private string name;
}
```

### 🔍 What's Happening Here?

- `ObservablePropertyAttribute` automatically generates the `Name` property, including `INotifyPropertyChanged` support.
- `RelayCommand` generates a `SayHelloCommand`, which can be bound to a button in the UI.

### 🖥️ XAML Binding Example

```xml
<UserControl xmlns:local="clr-namespace:MyApp.MyUserControl">
    <UserControl.DataContext>
        <local:TestViewModel/>
    </UserControl.DataContext>

    <StackPanel>

        <TextBox Text="{Binding Path=Name, UpdateSourceTrigger=PropertyChanged}" />

        <Button Content="Say Hello" Command="{Binding Path=SayHelloCommand}" />

    </StackPanel>
</UserControl>
```

This setup allows the UI to dynamically update when the Name property changes.

---

## 📌 Attributes for Property Source Generation

The `ObservableProperty` attribute automatically generates properties from private fields, including `INotifyPropertyChanged` support.

### 🛠 Quick Start: Using `ObservableProperty`

```csharp
// Generates a property named "Name"
[ObservableProperty()] private string name;

// Generates a property named "MyName"
[ObservableProperty("MyName")] private string name;

// Generates a property named "MyName" and notifies FullName and Age
[ObservableProperty(nameof(MyName), nameof(FullName), nameof(Age))] private string name;

// Generates a property named "MyName" and notifies FullName and Age
[ObservableProperty(nameof(MyName), DependentProperties = [nameof(FullName), nameof(Age)])] private string name;
```

### 🔔 Notifying Other Properties

```csharp
// Generates a property named "Name" and notifies FullName and Age
[ObservableProperty(DependentProperties = [nameof(FullName), nameof(Age)])]

// Notifies the property "Email"
[NotifyPropertyChangedFor(nameof(Email))]

// Notifies multiple properties
[NotifyPropertyChangedFor(nameof(FullName), nameof(Age))]
```

**Note:**

- `ObservableProperty` creates a public property from a private field and implements change notification.

- `NotifyPropertyChangedFor` ensures that when the annotated property changes, specified dependent properties also get notified.

## ⚡ Attributes for `RelayCommand` Source-Generation

The `RelayCommand` attribute generates `IRelayCommand` properties, eliminating manual command setup.

### 🛠 Quick Start Tips for RelayCommands

```csharp
// Generates a RelayCommand named "SaveCommand"
[RelayCommand()] public void Save();

// Generates a RelayCommand named "MySaveCommand"
[RelayCommand("MySave")] public void Save();
```

### 🏷️ Commands with CanExecute Logic

```csharp
// Generates a RelayCommand that takes a string parameter
[RelayCommand()] public void Save(string text);

// Generates a RelayCommand with CanExecute function
[RelayCommand(CanExecute = nameof(CanSave))] public void Save();
```

### 🔄 Asynchronous Commands

```csharp
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
```

### 🔁 Multi-Parameter Commands

```csharp
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

**Note:**

- The `RelayCommand` attribute generates an `IRelayCommand` pproperty linked to the annotated method.
- `CanExecute` logic can be specified to control when the command is executable.

---

## 🎯 Real-World Use Cases

### 📅 Scenario 1: A User Profile Form

```csharp
public partial class UserProfileViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullName))]
    private string firstName;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullName))]
    private string lastName;

    public string FullName => $"{FirstName} {LastName}";

    [RelayCommand]
    private void SaveProfile()
    {
        MessageBox.Show($"Profile Saved: {FullName}");
    }
}
```

#### 🔗 XAML Binding where Context is UserProfileViewModel

```xml
<TextBox Text="{Binding FirstName, UpdateSourceTrigger=PropertyChanged}" />
<TextBox Text="{Binding LastName, UpdateSourceTrigger=PropertyChanged}" />
<TextBlock Text="{Binding FullName}" />
<Button Content="Save" Command="{Binding SaveProfileCommand}" />
```

#### 🔥 Result for UserProfileViewModel binding

The FullName property updates automatically when FirstName or LastName changes

### 📑 Scenario 2: Fetching Data from an API

A ViewModel that fetches data asynchronously and enables/disables a button based on loading state.

```csharp
public partial class DataViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? data;

    [ObservableProperty]
    private bool isLoading;

    [RelayCommand(CanExecute = nameof(CanFetchData))]
    private async Task FetchData(CancellationToken cancellationToken)
    {
        IsLoading = true;
        await Task.Delay(2000, cancellationToken).ConfigureAwait(false); // Simulate API call
        Data = "Fetched Data from API";
        IsLoading = false;
    }

    private bool CanFetchData() => !IsLoading;
}
```

#### 🔗 XAML Binding where Context is DataViewModel

```xml
<Button Command="{Binding Path=FetchDataCommand}" Content="Fetch Data" />

<TextBlock Text="{Binding Path=Data}" />
```

#### 🔥 Result for DataViewModel binding

The button is disabled while data is being fetched, preventing multiple API calls.

## 🛠 Troubleshooting

### 🚧 Properties Are Not Updating in UI?

✅ Ensure your ViewModel inherits from ViewModelBase, which includes INotifyPropertyChanged.

```csharp
public partial class MyViewModel : ViewModelBase { }
```

### 🚧 Commands Are Not Executing?

✅ Check if your command has a valid CanExecute method.

```csharp
[RelayCommand(CanExecute = nameof(CanSave))]
private void Save() { /* ... */ }

private bool CanSave() => !string.IsNullOrEmpty(Name);
```

---

## 📌 Summary

- ✔️ **Use** `ObservableProperty` to eliminate manual property creation.
- ✔️ **Use** `NotifyPropertyChangedFor` to notify dependent properties.
- ✔️ **Use** `RelayCommand` for automatic command generation.
- ✔️ **Use async commands** for better UI responsiveness.
- ✔️ **Improve performance** by leveraging `CanExecute` for commands.

### 🚀 Why Use Atc.Wpf Source Generators?

- ✅ **Reduces boilerplate** – Write less code, get more done.
- ✅ **Improves maintainability** – Focus on business logic instead of plumbing.
- ✅ **Enhances MVVM architecture** – Ensures best practices in WPF development.

---

## 🔎 Behind the scenes

### 📝 Human-Written Code - for simple example

```csharp
public partial class TestViewModel : ViewModelBase
{
    [ObservableProperty]
    private string name;
}
```

### ⚙️ Auto-Generated Code - for simple example

```csharp
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

### 📝 Human-Written Code - for advanced example

```csharp
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

    [ObservableProperty(nameof(TheProperty), nameof(FullName), nameof(Age))]
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
}
```

### ⚙️ Auto-Generated Code - for advanced example

```csharp
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
