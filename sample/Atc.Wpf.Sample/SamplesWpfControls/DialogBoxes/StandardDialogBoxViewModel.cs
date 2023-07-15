// ReSharper disable UnusedVariable
namespace Atc.Wpf.Sample.SamplesWpfControls.DialogBoxes;

[SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "OK.")]
[SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed", Justification = "OK.")]
public class StandardDialogBoxViewModel : ViewModelBase
{
    private static readonly JsonSerializerOptions JsonOptions = Serialization.JsonSerializerOptionsFactory.Create();
    private string jsonResult = string.Empty;

    public IRelayCommand ShowInfoDialogBoxCommand => new RelayCommand(ShowInfoDialogBoxCommandHandler);

    public IRelayCommand ShowQuestionDialogBoxCommand => new RelayCommand(ShowQuestionDialogBoxCommandHandler);

    public IRelayCommand ShowInputDialogBoxCommand => new RelayCommand(ShowInputDialogBoxCommandHandler);

    public IRelayCommand ShowInputForm1ColumnDialogBoxCommand => new RelayCommand(ShowInputForm1ColumnDialogBoxCommandHandler);

    public IRelayCommand ShowInputForm2ColumnsDialogBoxCommand => new RelayCommand(ShowInputForm2ColumnsDialogBoxCommandHandler);

    public IRelayCommand ShowInputForm3ColumnsDialogBoxCommand => new RelayCommand(ShowInputForm3ColumnsDialogBoxCommandHandler);

    public IRelayCommand ShowInputFormAddressWithDataDialogBoxCommands => new RelayCommand(ShowInputFormAddressWithDataDialogBoxCommandHandler);

    public IRelayCommand ShowInputFormAddressWithoutDataDialogBoxCommands => new RelayCommand(ShowInputFormAddressWithoutDataDialogBoxCommandHandler);

    public IRelayCommand ShowInputFormPersonWithDataDialogBoxCommands => new RelayCommand(ShowInputFormPersonWithDataDialogBoxCommandHandler);

    public IRelayCommand ShowInputFormPersonWithoutDataDialogBoxCommands => new RelayCommand(ShowInputFormPersonWithoutDataDialogBoxCommandHandler);

    public string JsonResult
    {
        get => jsonResult;
        set
        {
            jsonResult = value;
            RaisePropertyChanged();
        }
    }

    private void ShowInfoDialogBoxCommandHandler()
    {
        var dialogBox = new InfoDialogBox(
            Application.Current.MainWindow!,
            "Hello world");

        var dialogResult = dialogBox.ShowDialog();
        JsonResult = dialogResult.HasValue && dialogResult.Value
            ? CreateJson(dialogBox.Settings.AffirmativeButtonText)
            : CreateJson(dialogBox.Settings.NegativeButtonText);
    }

    private void ShowQuestionDialogBoxCommandHandler()
    {
        var dialogBox = new QuestionDialogBox(
            Application.Current.MainWindow!,
            "Are you sure to delete item?");

        var dialogResult = dialogBox.ShowDialog();
        JsonResult = dialogResult.HasValue && dialogResult.Value
            ? CreateJson(dialogBox.Settings.AffirmativeButtonText)
            : CreateJson(dialogBox.Settings.NegativeButtonText);
    }

    private void ShowInputDialogBoxCommandHandler()
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
        if (dialogResult.HasValue && dialogResult.Value)
        {
            var newText = ((LabelTextBox)dialogBox.Data).Text;
            JsonResult = CreateJson(newText);
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputForm1ColumnDialogBoxCommandHandler()
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
        if (dialogResult.HasValue && dialogResult.Value)
        {
            var data = dialogBox.Data.GetKeyValues();
            JsonResult = JsonSerializer.Serialize(data, JsonOptions);
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputForm2ColumnsDialogBoxCommandHandler()
    {
        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(CreateLabelControlsColumn1());
        labelControlsForm.AddColumn(CreateLabelControlsColumn2());

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            var data = dialogBox.Data.GetKeyValues();
            JsonResult = JsonSerializer.Serialize(data, JsonOptions);
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputForm3ColumnsDialogBoxCommandHandler()
    {
        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(CreateLabelControlsColumn1());
        labelControlsForm.AddColumn(CreateLabelControlsColumn2());
        labelControlsForm.AddColumn(CreateLabelControlsColumn3());

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            var data = dialogBox.Data.GetKeyValues();
            JsonResult = JsonSerializer.Serialize(data, JsonOptions);
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputFormAddressWithDataDialogBoxCommandHandler()
    {
        var address = new Address(
            StreetName: "My street",
            CityName: "My city",
            PostalCode: "1234",
            Country: new CultureInfo("da-DK"));

        var labelControls = ModelToLabelControlHelper.GetLabelControls(address);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            var data = dialogBox.Data.GetKeyValues();
            JsonResult = JsonSerializer.Serialize(data, JsonOptions);
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputFormAddressWithoutDataDialogBoxCommandHandler()
    {
        var address = new Address(
            StreetName: string.Empty,
            CityName: string.Empty,
            PostalCode: string.Empty,
            Country: new CultureInfo("en-US"));

        var labelControls = ModelToLabelControlHelper.GetLabelControls(address);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            var data = dialogBox.Data.GetKeyValues();
            JsonResult = JsonSerializer.Serialize(data, JsonOptions);
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputFormPersonWithDataDialogBoxCommandHandler()
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
        if (dialogResult.HasValue && dialogResult.Value)
        {
            var data = dialogBox.Data.GetKeyValues();
            JsonResult = JsonSerializer.Serialize(data, JsonOptions);
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputFormPersonWithoutDataDialogBoxCommandHandler()
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
        if (dialogResult.HasValue && dialogResult.Value)
        {
            var data = dialogBox.Data.GetKeyValues();
            JsonResult = JsonSerializer.Serialize(data, JsonOptions);
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
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

    private string CreateJson(
        string value)
        => JsonResult = JsonSerializer.Serialize(value);
}