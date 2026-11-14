namespace Atc.Wpf.Sample.SamplesWpfComponents.DialogBoxes;

/// <summary>
/// Awaits every dialog of an <see cref="InWindowDialogService"/>; the view model only knows
/// <see cref="IDialogService"/> for the standard dialogs, as it would with the windowed <see cref="DialogService"/>.
/// </summary>
public sealed partial class InWindowDialogsViewModel : ViewModelBase
{
    private readonly InWindowDialogService dialogService;

    [ObservableProperty]
    private string result = "No dialog shown yet.";

    public InWindowDialogsViewModel(InWindowDialogService dialogService)
        => this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

    [RelayCommand]
    private async Task ShowInformation()
        => Result = $"Information: {await dialogService.ShowInformation("Information", "The report has been saved.").ConfigureAwait(true)}";

    [RelayCommand]
    private async Task ShowWarning()
        => Result = $"Warning: {await dialogService.ShowWarning("Warning", "The disk is almost full.").ConfigureAwait(true)}";

    [RelayCommand]
    private async Task ShowError()
        => Result = $"Error: {await dialogService.ShowError("Error", "The file could not be saved.").ConfigureAwait(true)}";

    [RelayCommand]
    private async Task ShowConfirmation()
        => Result = $"Confirmation: {await dialogService.ShowConfirmation("Delete file", "Do you want to delete 'report.pdf'?").ConfigureAwait(true)}";

    [RelayCommand]
    private async Task ShowOkCancel()
        => Result = $"OK / Cancel: {await dialogService.ShowOkCancel("Save changes", "Save the changes before closing?").ConfigureAwait(true)}";

    [RelayCommand]
    private async Task ShowInput()
    {
        var name = await dialogService.ShowInput("Rename", "New name", "report.pdf").ConfigureAwait(true);
        Result = name is null
            ? "Text input: cancelled"
            : $"Text input: {name}";
    }

    [RelayCommand]
    private async Task ShowInputForm()
    {
        var name = new LabelTextBox { LabelText = "Name", IsMandatory = true };
        var email = new LabelTextBox { LabelText = "E-mail" };
        var form = new LabelControlsForm();
        form.AddColumn(new List<ILabelControlBase> { name, email });

        Result = await dialogService.ShowInputForm("New contact", form).ConfigureAwait(true)
            ? $"Input form: {name.Text} <{email.Text}>"
            : "Input form: cancelled";
    }

    [RelayCommand]
    private async Task ShowColorPicker()
    {
        var color = await dialogService.ShowColorPicker("Pick a colour", Colors.SteelBlue).ConfigureAwait(true);
        Result = color is null
            ? "Colour picker: cancelled"
            : $"Colour picker: {color}";
    }

    [RelayCommand]
    private async Task ShowFontPicker()
    {
        var font = await dialogService.ShowFontPicker("Pick a font").ConfigureAwait(true);
        Result = font is null
            ? "Font picker: cancelled"
            : $"Font picker: {font.Family.Source}, {font.Size}";
    }
}