namespace Atc.Wpf.Sample.SourceWpfSourceGenerators;

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
    private string? lastName;

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
        var dialogBox = new InfoDialogBox(
            Application.Current.MainWindow!,
            new DialogBoxSettings(DialogBoxType.Ok),
            $"Data: {FullName}");

        dialogBox.ShowDialog();
    }

    [RelayCommand(CanExecute = nameof(CanSaveHandler))]
    public async Task SaveHandler(CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken).ConfigureAwait(false);

        await Application.Current.Dispatcher.BeginInvoke(() =>
        {
            var dialogBox = new InfoDialogBox(
                Application.Current.MainWindow!,
                new DialogBoxSettings(DialogBoxType.Ok),
                "Hello from SaveHandler method");

            dialogBox.ShowDialog();
        });
    }

    public bool CanSaveHandler()
        => !string.IsNullOrEmpty(FirstName) &&
           !string.IsNullOrEmpty(LastName) &&
           Age > 0;
}