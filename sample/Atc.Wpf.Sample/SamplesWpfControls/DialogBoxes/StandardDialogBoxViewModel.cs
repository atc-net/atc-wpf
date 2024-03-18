// ReSharper disable UnusedVariable
namespace Atc.Wpf.Sample.SamplesWpfControls.DialogBoxes;

public class StandardDialogBoxViewModel : ViewModelBase
{
    private readonly JsonSerializerOptions? jsonOptions;
    private string jsonResult = string.Empty;
    private bool showResultAsKeyValues;

    public StandardDialogBoxViewModel()
    {
        jsonOptions = Atc.Serialization.JsonSerializerOptionsFactory.Create();
        jsonOptions.Converters.Add(new JsonCultureInfoToNameConverter());
        jsonOptions.Converters.Add(new JsonColorToNameConverter());
    }

    public IRelayCommand ShowInfoDialogBoxCommand => new RelayCommand(ShowInfoDialogBoxCommandHandler);

    public IRelayCommand ShowInfoWarningDialogBoxCommand => new RelayCommand(ShowInfoWarningDialogBoxCommandHandler);

    public IRelayCommand ShowInfoErrorDialogBoxCommand => new RelayCommand(ShowInfoErrorDialogBoxCommandHandler);

    public IRelayCommand ShowQuestionDialogBoxCommand => new RelayCommand(ShowQuestionDialogBoxCommandHandler);

    public IRelayCommand ShowInputDialogBoxCommand => new RelayCommand(ShowInputDialogBoxCommandHandler);

    public IRelayCommand<int> ShowInputForm1ColumnXFieldVerticalDialogBoxCommand => new RelayCommand<int>(ShowInputForm1ColumnXFieldVerticalDialogBoxCommandHandler);

    public IRelayCommand<int> ShowInputForm1ColumnXFieldHorizontalDialogBoxCommand => new RelayCommand<int>(ShowInputForm1ColumnXFieldHorizontalDialogBoxCommandHandler);

    public IRelayCommand ShowInputForm1ColumnDialogBoxCommand => new RelayCommand(ShowInputForm1ColumnDialogBoxCommandHandler);

    public IRelayCommand ShowInputForm2ColumnsDialogBoxCommand => new RelayCommand(ShowInputForm2ColumnsDialogBoxCommandHandler);

    public IRelayCommand ShowInputForm3ColumnsDialogBoxCommand => new RelayCommand(ShowInputForm3ColumnsDialogBoxCommandHandler);

    public IRelayCommand ShowInputFormAddressWithDataDialogBoxCommands => new RelayCommand(ShowInputFormAddressWithDataDialogBoxCommandHandler);

    public IRelayCommand ShowInputFormAddressWithoutDataDialogBoxCommands => new RelayCommand(ShowInputFormAddressWithoutDataDialogBoxCommandHandler);

    public IRelayCommand ShowInputFormPersonWithDataDialogBoxCommands => new RelayCommand(ShowInputFormPersonWithDataDialogBoxCommandHandler);

    public IRelayCommand ShowInputFormPersonWithoutDataDialogBoxCommands => new RelayCommand(ShowInputFormPersonWithoutDataDialogBoxCommandHandler);

    public IRelayCommand ShowInputFormAccountWithDataDialogBoxCommands => new RelayCommand(ShowInputFormAccountWithDataDialogBoxCommandHandler);

    public IRelayCommand ShowInputFormAccountWithoutDataDialogBoxCommands => new RelayCommand(ShowInputFormAccountWithoutDataDialogBoxCommandHandler);

    public IRelayCommand ShowInputForm1ColumnByLabelControlsDataDialogBoxCommand => new RelayCommand(ShowInputForm1ColumnByLabelControlsDataDialogBoxCommandHandler);
    public IRelayCommand ShowBasicApplicationSettingsDialogBoxCommand => new RelayCommand(ShowBasicApplicationSettingsDialogBoxCommandHandler);

    public bool ShowResultAsKeyValues
    {
        get => showResultAsKeyValues;
        set
        {
            showResultAsKeyValues = value;
            RaisePropertyChanged();
        }
    }

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
            new DialogBoxSettings(DialogBoxType.Ok)
            {
                TitleBarText = "This is a information",
            },
            "Hello information");

        var dialogResult = dialogBox.ShowDialog();
        JsonResult = dialogResult.HasValue && dialogResult.Value
            ? CreateJson(dialogBox.Settings.AffirmativeButtonText)
            : CreateJson(dialogBox.Settings.NegativeButtonText);
    }

    private void ShowInfoWarningDialogBoxCommandHandler()
    {
        var dialogBox = new InfoDialogBox(
            Application.Current.MainWindow!,
            new DialogBoxSettings(DialogBoxType.Ok, LogCategoryType.Warning)
            {
                TitleBarText = "This is a warning",
            },
            "Hello warning");

        var dialogResult = dialogBox.ShowDialog();
        JsonResult = dialogResult.HasValue && dialogResult.Value
            ? CreateJson(dialogBox.Settings.AffirmativeButtonText)
            : CreateJson(dialogBox.Settings.NegativeButtonText);
    }

    private void ShowInfoErrorDialogBoxCommandHandler()
    {
        var dialogBox = new InfoDialogBox(
            Application.Current.MainWindow!,
            new DialogBoxSettings(DialogBoxType.Ok, LogCategoryType.Error)
            {
                TitleBarText = "This is a error",
            },
            "Hello error");

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

    private void ShowInputForm1ColumnXFieldVerticalDialogBoxCommandHandler(
        int numberOfControls)
    {
        var labelControls = new List<ILabelControlBase>();

        for (var i = 0; i < numberOfControls; i++)
        {
            labelControls.Add(
                new LabelTextBox
                {
                    LabelText = $"Name{i}",
                });
        }

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        dialogBox.Settings.Form.ControlOrientation = Orientation.Vertical;
        dialogBox.LabelInputFormPanel.ReRender();

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            var data = dialogBox.Data.GetKeyValues();
            JsonResult = JsonSerializer.Serialize(data, jsonOptions);
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputForm1ColumnXFieldHorizontalDialogBoxCommandHandler(
        int numberOfControls)
    {
        var labelControls = new List<ILabelControlBase>();

        for (var i = 0; i < numberOfControls; i++)
        {
            labelControls.Add(
                new LabelTextBox
                {
                    LabelText = $"Name{i}",
                });
        }

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        dialogBox.Settings.Form.ControlOrientation = Orientation.Horizontal;
        dialogBox.ReRender();

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            var data = dialogBox.Data.GetKeyValues();
            JsonResult = JsonSerializer.Serialize(data, jsonOptions);
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
            JsonResult = JsonSerializer.Serialize(data, jsonOptions);
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
            JsonResult = JsonSerializer.Serialize(data, jsonOptions);
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
            JsonResult = JsonSerializer.Serialize(data, jsonOptions);
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputFormAddressWithDataDialogBoxCommandHandler()
    {
        var address = new Address(
            streetName: "My street",
            cityName: "My city",
            postalCode: "1234",
            country: new CultureInfo("da-DK"));

        var labelControls = ModelToLabelControlExtractor.Extract(address);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            if (ShowResultAsKeyValues)
            {
                var data = dialogBox.Data.GetKeyValues();
                JsonResult = JsonSerializer.Serialize(data, jsonOptions);
            }
            else
            {
                var data = LabelControlsFormToModelWriter.Update(address, dialogBox.Data);
                JsonResult = JsonSerializer.Serialize(data, jsonOptions);
            }
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputFormAddressWithoutDataDialogBoxCommandHandler()
    {
        var address = new Address(
            streetName: string.Empty,
            cityName: string.Empty,
            postalCode: string.Empty,
            country: new CultureInfo("en-US"));

        var labelControls = ModelToLabelControlExtractor.Extract(address);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            if (ShowResultAsKeyValues)
            {
                var data = dialogBox.Data.GetKeyValues();
                JsonResult = JsonSerializer.Serialize(data, jsonOptions);
            }
            else
            {
                var data = LabelControlsFormToModelWriter.Update(address, dialogBox.Data);
                JsonResult = JsonSerializer.Serialize(data, jsonOptions);
            }
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputFormPersonWithDataDialogBoxCommandHandler()
    {
        var person = new Person(
            firstName: "John",
            lastName: "Doe",
            age: 33,
            favoriteColor: Colors.DarkCyan,
            myAddress: new Address(
                streetName: "My street",
                cityName: "My city",
                postalCode: "1234",
                country: new CultureInfo("da-DK")));

        var labelControls = ModelToLabelControlExtractor.Extract(
            person,
            includeReadOnly: true,
            groupIdentifier: "MyPerson");

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            if (ShowResultAsKeyValues)
            {
                var data = dialogBox.Data.GetKeyValues();
                JsonResult = JsonSerializer.Serialize(data, jsonOptions);
            }
            else
            {
                var data = LabelControlsFormToModelWriter.Update(person, dialogBox.Data);
                JsonResult = JsonSerializer.Serialize(data, jsonOptions);
            }
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputFormPersonWithoutDataDialogBoxCommandHandler()
    {
        var person = new Person(
            firstName: string.Empty,
            lastName: string.Empty,
            age: 0,
            favoriteColor: Colors.Aqua,
            myAddress: new Address(
                streetName: string.Empty,
                cityName: string.Empty,
                postalCode: string.Empty,
                country: new CultureInfo("en-US")));

        var labelControls = ModelToLabelControlExtractor.Extract(person);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            if (ShowResultAsKeyValues)
            {
                var data = dialogBox.Data.GetKeyValues();
                JsonResult = JsonSerializer.Serialize(data, jsonOptions);
            }
            else
            {
                var data = LabelControlsFormToModelWriter.Update(person, dialogBox.Data);
                JsonResult = JsonSerializer.Serialize(data, jsonOptions);
            }
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputFormAccountWithDataDialogBoxCommandHandler()
    {
        var account = new Account(
            accountNumber: "6545643218",
            createdDate: DateTime.Now.AddYears(-2).AddDays(3),
            primaryContactPerson: new Person(
                firstName: "John",
                lastName: "Doe",
                age: 33,
                favoriteColor: Colors.DarkCyan,
                myAddress: new Address(
                    streetName: "My street",
                    cityName: "My city",
                    postalCode: "1234",
                    country: new CultureInfo("da-DK"))));

        var labelControls = ModelToLabelControlExtractor.Extract(account);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            if (ShowResultAsKeyValues)
            {
                var data = dialogBox.Data.GetKeyValues();
                JsonResult = JsonSerializer.Serialize(data, jsonOptions);
            }
            else
            {
                var data = LabelControlsFormToModelWriter.Update(account, dialogBox.Data);
                JsonResult = JsonSerializer.Serialize(data, jsonOptions);
            }
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputFormAccountWithoutDataDialogBoxCommandHandler()
    {
        var account = new Account(
            accountNumber: string.Empty,
            createdDate: DateTime.Now.AddYears(-2).AddDays(3),
            primaryContactPerson: new Person(
                firstName: string.Empty,
                lastName: string.Empty,
                age: 0,
                favoriteColor: Colors.Aqua,
                myAddress: new Address(
                    streetName: string.Empty,
                    cityName: string.Empty,
                    postalCode: string.Empty,
                    country: new CultureInfo("en-US"))));

        var labelControls = ModelToLabelControlExtractor.Extract(account);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            if (ShowResultAsKeyValues)
            {
                var data = dialogBox.Data.GetKeyValues();
                JsonResult = JsonSerializer.Serialize(data, jsonOptions);
            }
            else
            {
                var data = LabelControlsFormToModelWriter.Update(account, dialogBox.Data);
                JsonResult = JsonSerializer.Serialize(data, jsonOptions);
            }
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowInputForm1ColumnByLabelControlsDataDialogBoxCommandHandler()
    {
        var listOfLabelControlsData = new List<LabelControlData>()
        {
            new(
                dataType: typeof(string),
                labelText: "MyFirstName")
            {
                IsMandatory = true,
            },
            new(
                dataType: typeof(string),
                labelText: "MyLastName")
            {
                IsMandatory = true,
                Minimum = 2,
            },
            new(
                dataType: typeof(int),
                labelText: "Age")
            {
                Minimum = 0,
            },
            new(
                dataType: typeof(string),
                labelText: "Note"),
        };

        var labelControls = LabelControlDataToLabelControlExtractor.Extract(listOfLabelControlsData);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var dialogBox = new InputFormDialogBox(
            Application.Current.MainWindow!,
            labelControlsForm);

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            var data = dialogBox.Data.GetKeyValues();
            JsonResult = JsonSerializer.Serialize(data, jsonOptions);
        }
        else
        {
            JsonResult = CreateJson(dialogBox.Settings.NegativeButtonText);
        }
    }

    private void ShowBasicApplicationSettingsDialogBoxCommandHandler()
    {
        var dialogBox = new BasicApplicationSettingsDialogBox
        {
            DataContext = new BasicApplicationSettingsDialogBoxViewModel(
                new DirectoryInfo(@"C:\Temp"),
                new BasicApplicationSettingsViewModel(
                    new BasicApplicationOptions
                    {
                        Theme = ThemeManager.Current.DetectTheme(Application.Current)!.Name,
                        Language = CultureManager.UiCulture.Name,
                        OpenRecentFileOnStartup = true,
                    })),
        };

        var dialogResult = dialogBox.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            JsonResult = dialogBox.GetDataAsJson();
        }
        else
        {
            JsonResult = "{}";
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
        => JsonResult = $"{{ \"Value\": \"{value}\" }}";
}