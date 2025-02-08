namespace Atc.Wpf.Sample.SamplesWpf.Mvvm;

[SuppressMessage("Maintainability", "CA1507:Use nameof to express symbol names", Justification = "OK.")]
public partial class PersonViewModel : ViewModelBase
{
    [ObservableProperty]
    [AlsoNotifyProperty(nameof(FullName))]
    private string firstName = "John";

    [ObservableProperty]
    [AlsoNotifyProperty(nameof(FullName), nameof(Age))]
    [AlsoNotifyProperty(nameof(Email))]
    [AlsoNotifyProperty(nameof(TheProperty))]
    private string? lastName = "Doe";

    [ObservableProperty]
    private int age = 27;

    [ObservableProperty]
    private string? email;

    [ObservableProperty("TheProperty", nameof(FullName), nameof(Age))]
    private string? myTestProperty;

    public string FullName => $"{FirstName} {LastName}";
}