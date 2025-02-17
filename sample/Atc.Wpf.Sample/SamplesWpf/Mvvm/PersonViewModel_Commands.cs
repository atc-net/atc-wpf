namespace Atc.Wpf.Sample.SamplesWpf.Mvvm;

public partial class PersonViewModel
{
    [RelayCommand]
    public void ShowData()
    {
        var sb = new StringBuilder();
        sb.Append("FirstName: ");
        sb.AppendLine(FirstName);
        sb.Append("LastName: ");
        sb.AppendLine(LastName);
        sb.Append("Age: ");
        sb.AppendLine(Age.ToString(GlobalizationConstants.EnglishCultureInfo));
        sb.Append("Email: ");
        sb.AppendLine(Email);
        sb.Append("TheProperty: ");
        sb.AppendLine(TheProperty);

        var dialogBox = new InfoDialogBox(
            Application.Current.MainWindow!,
            new DialogBoxSettings(DialogBoxType.Ok)
            {
                Height = 300,
            },
            sb.ToString());

        dialogBox.ShowDialog();
    }

    [RelayCommand(CanExecute = nameof(CanSaveHandler))]
    public void SaveHandler()
    {
        var dialogBox = new InfoDialogBox(
            Application.Current.MainWindow!,
            new DialogBoxSettings(DialogBoxType.Ok),
            "Hello from SaveHandler method");

        dialogBox.ShowDialog();
    }

    public bool CanSaveHandler()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            return false;
        }

        if (Age is < 18 or > 120)
        {
            return false;
        }

        if (Email is not null && !Email.IsEmailAddress())
        {
            return false;
        }

        return true;
    }
}