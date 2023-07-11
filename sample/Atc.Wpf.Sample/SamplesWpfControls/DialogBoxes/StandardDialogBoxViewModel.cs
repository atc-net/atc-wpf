// ReSharper disable UnusedVariable
namespace Atc.Wpf.Sample.SamplesWpfControls.DialogBoxes;

[SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "OK.")]
[SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed", Justification = "OK.")]
public class StandardDialogBoxViewModel : ViewModelBase
{
    public static IRelayCommand ShowInfoDialogBoxCommand => new RelayCommand(ShowInfoDialogBoxCommandHandler);

    public static IRelayCommand ShowQuestionDialogBoxCommand => new RelayCommand(ShowQuestionDialogBoxCommandHandler);

    public static IRelayCommand ShowInputDialogBoxCommand => new RelayCommand(ShowInputDialogBoxCommandHandler);

    public static IRelayCommand ShowInputForm1ColumnDialogBoxCommand => new RelayCommand(ShowInputForm1ColumnDialogBoxCommandHandler);

    public static IRelayCommand ShowInputForm2ColumnsDialogBoxCommand => new RelayCommand(ShowInputForm2ColumnsDialogBoxCommandHandler);

    public static IRelayCommand ShowInputForm3ColumnsDialogBoxCommand => new RelayCommand(ShowInputForm3ColumnsDialogBoxCommandHandler);

    public static IRelayCommand ShowInputFormPersonWithDataDialogBoxCommands => new RelayCommand(ShowInputFormPersonWithDataDialogBoxCommandHandler);

    public static IRelayCommand ShowInputFormPersonWithoutDataDialogBoxCommands => new RelayCommand(ShowInputFormPersonWithoutDataDialogBoxCommandHandler);

    private static void ShowInfoDialogBoxCommandHandler()
    {
        var dialogBox = new InfoDialogBox(
            Application.Current.MainWindow!,
            "Hello world");

        var dialogResult = dialogBox.ShowDialog();
    }

    private static void ShowQuestionDialogBoxCommandHandler()
    {
        var dialogBox = new QuestionDialogBox(
            Application.Current.MainWindow!,
            "Are you sure to delete item?");

        var dialogResult = dialogBox.ShowDialog();
    }

    private static void ShowInputDialogBoxCommandHandler()
    {
        var dialogBox = new InputDialogBox(
            Application.Current.MainWindow!,
            new LabelTextBox
            {
                LabelText = "Name",
                IsMandatory = true,
                MinLength = 2,
            });

        var dialogResult = dialogBox.ShowDialog();
    }

    private static void ShowInputForm1ColumnDialogBoxCommandHandler()
    {
        var labelControls = new List<ILabelControlBase>
        {
            new LabelTextBox
            {
                LabelText = "FirstName",
                IsMandatory = true,
                MinLength = 2,
            },
            new LabelTextBox
            {
                LabelText = "LastName",
                IsMandatory = true,
                MinLength = 2,
            },
            new LabelIntegerBox
            {
                LabelText = "Age",
                Minimum = 0,
            },
            new LabelTextBox
            {
                LabelText = "Note",
            },
        };

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
    }

    private static void ShowInputForm2ColumnsDialogBoxCommandHandler()
    {
        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(CreateLabelControlsColumn1());
        labelControlsForm.AddColumn(CreateLabelControlsColumn2());

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
    }

    private static void ShowInputForm3ColumnsDialogBoxCommandHandler()
    {
        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(CreateLabelControlsColumn1());
        labelControlsForm.AddColumn(CreateLabelControlsColumn2());
        labelControlsForm.AddColumn(CreateLabelControlsColumn3());

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
    }

    private static void ShowInputFormPersonWithDataDialogBoxCommandHandler()
    {
        var person = new Person(
            FirstName: "John",
            LastName: "Doe",
            Age: 33,
            Address: new Address(
                StreetName: "My street",
                CityName: "My city",
                PostalCode: "1234",
                Country: new CultureInfo("da-DK")));

        var labelControls = ModelToLabelControlHelper.GetLabelControls(person);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
    }

    private static void ShowInputFormPersonWithoutDataDialogBoxCommandHandler()
    {
        var person = new Person(
            FirstName: string.Empty,
            LastName: string.Empty,
            Age: 0,
            Address: new Address(
                StreetName: string.Empty,
                CityName: string.Empty,
                PostalCode: string.Empty,
                Country: new CultureInfo("en-US")));

        var labelControls = ModelToLabelControlHelper.GetLabelControls(person);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
    }

    private static List<ILabelControlBase> CreateLabelControlsColumn1()
        => new()
        {
            new LabelTextBox
            {
                LabelText = "FirstName",
                IsMandatory = true,
                MinLength = 2,
            },
            new LabelTextBox
            {
                LabelText = "LastName",
                IsMandatory = true,
                MinLength = 2,
            },
        };

    private static List<ILabelControlBase> CreateLabelControlsColumn2()
        => new()
        {
            new LabelIntegerBox
            {
                LabelText = "Age",
                Minimum = 0,
            },
            new LabelTextBox
            {
                LabelText = "Note",
            },
        };

    private static List<ILabelControlBase> CreateLabelControlsColumn3()
        => new()
        {
            new LabelCheckBox
            {
                LabelText = "Use Foo",
            },
            new LabelComboBox
            {
                LabelText = "Items",
                Items = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    { "Item1", "Item 1" },
                    { "Item2", "Item 2" },
                    { "Item3", "Item 3" },
                    { "Item4", "Item 4" },
                    { "Item5", "Item 5" },
                },
            },
        };
}