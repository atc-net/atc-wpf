// ReSharper disable UnusedVariable
namespace Atc.Wpf.Sample.SamplesWpfComponents.DialogBoxes;

public sealed partial class StandardDialogBoxViewModel : ViewModelBase
{
    private readonly JsonSerializerOptions? jsonOptions;
    private string jsonResult = string.Empty;
    private bool showResultAsKeyValues;

    public StandardDialogBoxViewModel()
    {
        jsonOptions = Atc.Serialization.JsonSerializerOptionsFactory.Create();
        jsonOptions.Converters.Add(new CultureInfoToNameJsonConverter());
        jsonOptions.Converters.Add(new ColorToNameJsonConverter());
    }

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

    [RelayCommand]
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

    [RelayCommand]
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

    [RelayCommand]
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

    [RelayCommand]
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

    [RelayCommand]
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

    [RelayCommand]
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

    [RelayCommand]
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

    [RelayCommand]
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

    [RelayCommand]
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

    [RelayCommand]
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

    [RelayCommand]
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

    [RelayCommand]
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

    [RelayCommand]
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

    [RelayCommand]
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

    [RelayCommand]
    private void ShowInputFormAccountWithDataDialogBoxCommandHandler()
    {
        var account = new Account(
            accountNumber: "6545643218",
            createdDate: DateTime.Now
                .AddYears(-2)
                .AddDays(3),
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

    [RelayCommand]
    private void ShowInputFormAccountWithoutDataDialogBoxCommandHandler()
    {
        var account = new Account(
            accountNumber: string.Empty,
            createdDate: DateTime.Now
                .AddYears(-2)
                .AddDays(3),
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

    [RelayCommand]
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

    [RelayCommand]
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
        =>
        [
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
        ];

    private static List<ILabelControlBase> CreateLabelControlsColumn2()
        =>
        [
            new LabelIntegerBox
            {
                LabelText = "Age",
                Minimum = 0,
            },
            new LabelTextBox
            {
                LabelText = "Note",
            },
        ];

    private static List<ILabelControlBase> CreateLabelControlsColumn3()
        =>
        [
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
        ];

    private string CreateJson(string value)
        => JsonResult = $"{{ \"Value\": \"{value}\" }}";
}