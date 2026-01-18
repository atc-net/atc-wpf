namespace Atc.Wpf.Sample.SamplesWpf.Dialogs;

public sealed partial class DialogServiceViewModel : ViewModelBase
{
    private readonly IDialogService dialogService = new DialogService();
    private string lastResult = "(none)";
    private Color? selectedColor;

    public string LastResult
    {
        get => lastResult;
        set
        {
            lastResult = value;
            RaisePropertyChanged();
        }
    }

    public Color? SelectedColor
    {
        get => selectedColor;
        set
        {
            selectedColor = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(SelectedColorBrush));
            RaisePropertyChanged(nameof(HasSelectedColor));
        }
    }

    public SolidColorBrush? SelectedColorBrush
        => SelectedColor.HasValue
            ? new SolidColorBrush(SelectedColor.Value)
            : null;

    public bool HasSelectedColor => SelectedColor.HasValue;

    [RelayCommand]
    private async Task ShowInformation()
    {
        var result = await dialogService
            .ShowInformation(
                "Information",
                "This is an information message displayed using IDialogService.")
            .ConfigureAwait(false);

        LastResult = result
            ? "OK clicked"
            : "Dialog closed";
    }

    [RelayCommand]
    private async Task ShowWarning()
    {
        var result = await dialogService
            .ShowWarning(
                "Warning",
                "This is a warning message. Please pay attention!")
            .ConfigureAwait(false);

        LastResult = result
            ? "OK clicked"
            : "Dialog closed";
    }

    [RelayCommand]
    private async Task ShowError()
    {
        var result = await dialogService
            .ShowError(
                "Error",
                "An error has occurred. This is how error dialogs look.")
            .ConfigureAwait(false);

        LastResult = result
            ? "OK clicked"
            : "Dialog closed";
    }

    [RelayCommand]
    private async Task ShowConfirmation()
    {
        var result = await dialogService
            .ShowConfirmation(
                "Confirm Action",
                "Are you sure you want to proceed with this action?")
            .ConfigureAwait(false);

        LastResult = result
            ? "Yes clicked"
            : "No clicked";
    }

    [RelayCommand]
    private async Task ShowOkCancel()
    {
        var result = await dialogService
            .ShowOkCancel(
                "Confirm",
                "Do you want to save your changes?")
            .ConfigureAwait(false);
        LastResult = result
            ? "OK clicked"
            : "Cancel clicked";
    }

    [RelayCommand]
    private async Task ShowInput()
    {
        var result = await dialogService
            .ShowInput(
                "Enter Name",
                "Your name:",
                "John Doe")
            .ConfigureAwait(false);

        LastResult = result is not null
            ? $"Entered: {result}"
            : "Cancelled";
    }

    [RelayCommand]
    private async Task ShowColorPicker()
    {
        var result = await dialogService
            .ShowColorPicker(
                "Select Color",
                Colors.DodgerBlue)
            .ConfigureAwait(false);

        if (result.HasValue)
        {
            SelectedColor = result.Value;
            LastResult = $"Selected: {result.Value}";
        }
        else
        {
            LastResult = "Cancelled";
        }
    }
}